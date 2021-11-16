using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UI;

public class NetworkedServer : MonoBehaviour
{
    const int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    const int socketPort = 5491;

    LinkedList<PlayerAccount> playerAccounts;

    const int PlayerAccountNameAndPassword = 1;

    string playerAccountsFilePath;

    int playerWaitForMatchWithID = -1;
   

    LinkedList<GameRoom> gameRooms;

    

    void Start()
    {
        NetworkTransport.Init();
        ConnectionConfig config = new ConnectionConfig();

        reliableChannelID = config.AddChannel(QosType.Reliable);
        unreliableChannelID = config.AddChannel(QosType.Unreliable);
        HostTopology topology = new HostTopology(config, maxConnections);
        hostID = NetworkTransport.AddHost(topology, socketPort, null); //Server does not need to know IP, so this is NULL in this case

        playerAccountsFilePath = Application.dataPath + Path.DirectorySeparatorChar + "PlayerAccounts.txt";
        playerAccounts = new LinkedList<PlayerAccount>();

        LoadPlayerAccounts();

        
        //Testing if player accounts can be loaded
        foreach(PlayerAccount pa in playerAccounts)
        {
            Debug.Log(pa.name + " " + pa.password);
        }

        gameRooms = new LinkedList<GameRoom>();

    }

    // Update is called once per frame
    void Update()
    {

        int recHostID;
        int recConnectionID;
        int recChannelID;
        byte[] recBuffer = new byte[1024];
        int bufferSize = 1024;
        int dataSize;
        byte error = 0;

        NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

        switch (recNetworkEvent)
        {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("Connection, " + recConnectionID);
                break;
            case NetworkEventType.DataEvent:
                string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize); //recBuffer is a byte array being converted to string via Encoding.Unicode
                ProcessRecievedMsg(msg, recConnectionID);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("Disconnection, " + recConnectionID);
                break;
        }
        
    }
  
    public void SendMessageToClient(string msg, int id)
    {
        byte error = 0;
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, id, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }
    
    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');

        int signifier = int.Parse(csv[0]);

        if(signifier == ClientToServerSignifiers.CreateAccount)
        {
            Debug.Log("create account");

            string n = csv[1];
            string p = csv[2];
            bool nameIsInUse = false;

            foreach (PlayerAccount pa in playerAccounts)
            {
                if (pa.name == n)
                    nameIsInUse = true;
                    break;
            }

            if(nameIsInUse)
            {
                SendMessageToClient(ServerToClientSignifiers.AccountCreationFailed + "", id);
            }
            else
            {

                PlayerAccount newPlayerAccount = new PlayerAccount(n, p);
                playerAccounts.AddLast(newPlayerAccount);
                SendMessageToClient(ServerToClientSignifiers.AccountCreationComplete + "", id);

                SavePlayerAccounts();



            }
        }
        else if (signifier == ClientToServerSignifiers.Login)
        {
            Debug.Log("login to account");
            //check if player account name already exists,

            string n = csv[1];
            string p = csv[2];
            bool hasNameBeenFound = false;
            bool msgHasBeenSentToClient = false;


            foreach (PlayerAccount pa in playerAccounts)
            {
                if (pa.name == n)
                {

                    hasNameBeenFound = true;

                    if (pa.password == p)
                    {
                        //Success
                        SendMessageToClient(ServerToClientSignifiers.LoginComplete + "", id);
                        msgHasBeenSentToClient = true;
                    }
                    else
                    {
                        //Failure
                        SendMessageToClient(ServerToClientSignifiers.LoginFailed + "", id);
                        msgHasBeenSentToClient = true;
                    }
                }
            }

            if (!hasNameBeenFound)
            {
                if(!msgHasBeenSentToClient)
                {
                    SendMessageToClient(ServerToClientSignifiers.LoginFailed + "", id);
                }
            }

            //check if password matches
            //send to client to success/failure
        }
        else if (signifier == ClientToServerSignifiers.JoinQueueForGameRoom)
        {
            Debug.Log("We need to get this player into a waiting queue!");

            if (playerWaitForMatchWithID == -1)
            {
                playerWaitForMatchWithID = id;
            }
            else if (playerWaitForMatchWithID == -2)
            {
                GameRoom gr = GetGameRoomWithClientID(id-1);
                gr.observerID3 = id;
                SendMessageToClient(ServerToClientSignifiers.GameStart + "", gr.observerID3);
                playerWaitForMatchWithID = -1;
            }
            else
            {
               

                GameRoom gr = new GameRoom(playerWaitForMatchWithID, id);
                gameRooms.AddLast(gr);

                SendMessageToClient(ServerToClientSignifiers.GameStart + "", gr.playerID2);
                SendMessageToClient(ServerToClientSignifiers.GameStart + "", gr.playerID1);

                playerWaitForMatchWithID = -2;
            }
          
            
        }

        else if (signifier == ClientToServerSignifiers.TicTacToeSomethingSomethingPlay)
        {

            GameRoom gr = GetGameRoomWithClientID(id);

            if (gr != null)
            {
                if(gr.playerID1 == id)
                {
                    SendMessageToClient(ServerToClientSignifiers.OpponentPlay + "", gr.playerID2);
                }
                else
                    SendMessageToClient(ServerToClientSignifiers.OpponentPlay + "", gr.playerID1);
            }
        }

        else if (signifier == ClientToServerSignifiers.ClientToClientMsgSent)
        {

            GameRoom gr = GetGameRoomWithClientID(id);

            if (gr != null)
            {
                SendMessageToClient(ServerToClientSignifiers.ClientToClientMsgReceived + "," + csv[1], gr.playerID1);
                SendMessageToClient(ServerToClientSignifiers.ClientToClientMsgReceived + "," + csv[1], gr.playerID2);
                SendMessageToClient(ServerToClientSignifiers.ClientToClientMsgReceived + "," + csv[1], gr.observerID3);
            }
        }

        else if (signifier == ClientToServerSignifiers.ClientMoveSent)
        {
            GameRoom gr = GetGameRoomWithClientID(id);

            if (gr != null)
            {
                if (gr.playerID1 == id)
                {
                    SendMessageToClient(ServerToClientSignifiers.ClientMoveReceived + "," + csv[1] + ",2", gr.playerID2);
                    SendMessageToClient(ServerToClientSignifiers.ClientMoveReceived + "," + csv[1] + ",2", gr.playerID1);
                    SendMessageToClient(ServerToClientSignifiers.ClientMoveReceived + "," + csv[1] + ",2", gr.observerID3);
                    gr.movesMade++;
                    if (gr.movesMade ==9)
                    {
                        SendMessageToClient(ServerToClientSignifiers.WinnerTold + ",3", gr.playerID2);
                        SendMessageToClient(ServerToClientSignifiers.WinnerTold + ",3", gr.playerID1);
                        SendMessageToClient(ServerToClientSignifiers.WinnerTold + ",3", gr.observerID3);
                    }
                }
                else if (gr.playerID2 == id)
                {
                    SendMessageToClient(ServerToClientSignifiers.ClientMoveReceived + "," + csv[1] + ",1", gr.playerID1);
                    SendMessageToClient(ServerToClientSignifiers.ClientMoveReceived + "," + csv[1] + ",1", gr.playerID2);
                    SendMessageToClient(ServerToClientSignifiers.ClientMoveReceived + "," + csv[1] + ",1", gr.observerID3);
                    gr.movesMade++;

                    if (gr.movesMade == 9)
                    {
                        SendMessageToClient(ServerToClientSignifiers.WinnerTold + ",3", gr.playerID2);
                        SendMessageToClient(ServerToClientSignifiers.WinnerTold + ",3", gr.playerID1);
                        SendMessageToClient(ServerToClientSignifiers.WinnerTold + ",3", gr.observerID3);
                    }
                }
            }

        }

        else if (signifier == ClientToServerSignifiers.WinnerFound)
        {
            GameRoom gr = GetGameRoomWithClientID(id);

            if (gr != null)
            {
               if (int.Parse(csv[1]) == 1)
               {
                    SendMessageToClient(ServerToClientSignifiers.WinnerTold + ",1", gr.playerID2);
                    SendMessageToClient(ServerToClientSignifiers.WinnerTold + ",1", gr.playerID1);
                    SendMessageToClient(ServerToClientSignifiers.WinnerTold + ",1", gr.observerID3);
                }
               else if (int.Parse(csv[1])==2)
               {
                    SendMessageToClient(ServerToClientSignifiers.WinnerTold + ",2", gr.playerID2);
                    SendMessageToClient(ServerToClientSignifiers.WinnerTold + ",2", gr.playerID1);
                    SendMessageToClient(ServerToClientSignifiers.WinnerTold + ",2", gr.observerID3);
                }
                   
            }

        }

    }

    private void SavePlayerAccounts()
    {
        //WRITER
        StreamWriter sw = new StreamWriter(playerAccountsFilePath);

        foreach(PlayerAccount pa in playerAccounts)
        {
            sw.WriteLine(PlayerAccountNameAndPassword + "," + pa.name + "," + pa.password);
        }

        sw.Close();
    }

    private void LoadPlayerAccounts()
    {
        if (File.Exists(playerAccountsFilePath))
        {



            //READER
            StreamReader sr = new StreamReader(playerAccountsFilePath);

            string line;

            //while the line is still being read
            while (true)
            {
                line = sr.ReadLine();

                if (line == null)
                    break;


                string[] csv = line.Split(',');

                int signifier = int.Parse(csv[0]);

                if (signifier == PlayerAccountNameAndPassword)
                {
                    PlayerAccount pa = new PlayerAccount(csv[1], csv[2]);
                    playerAccounts.AddLast(pa);
                }
                //else if (signifier == )
                //{

                //}

            }

            sr.Close();
        }
    }

    private GameRoom GetGameRoomWithClientID(int id)
    {
        foreach (GameRoom gr in gameRooms)
        {
            if(gr.playerID1 == id || gr.playerID2 == id || gr.observerID3 == id)
            {
                return gr;
            }
        }

        return null;
    }
}


public class PlayerAccount
{
    public string name, password;

    public PlayerAccount(string Name, string Password)
    {
        name = Name;
        password = Password;
    }
}


public class GameRoom
{
    public int playerID1, playerID2, observerID3;
    public int movesMade = 0;
    public GameRoom(int PlayerID1, int PlayerID2)
    {
        playerID1 = PlayerID1;
        playerID2 = PlayerID2;
       
    }

}

public static class ClientToServerSignifiers
{
    public const int CreateAccount = 1;

    public const int Login = 2;

    public const int JoinQueueForGameRoom = 3;

    public const int TicTacToeSomethingSomethingPlay = 4;

    public const int ClientToClientMsgSent = 5;

    public const int ClientMoveSent = 6;

    public const int WinnerFound = 7; 
}

public static class ServerToClientSignifiers
{
    public const int LoginComplete = 1;

    public const int LoginFailed = 2;

    public const int AccountCreationComplete = 3;

    public const int AccountCreationFailed = 4;

    public const int OpponentPlay = 5;

    public const int GameStart = 6;

    public const int ClientToClientMsgReceived = 7;

    public const int ClientMoveReceived = 8;

    public const int WinnerTold = 9;

}
