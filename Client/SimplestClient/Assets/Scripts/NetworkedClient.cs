using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkedClient : MonoBehaviour
{

    int connectionID; 
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;
    byte error;
    bool isConnected = false;
    int ourClientID;
    private Text TextBox;
    public int[] moves;
    public int movesI = 0;

    GameObject gameSystemManager;

    // Start is called before the first frame update
    void Start()
    {
        TextBox = GameObject.Find("MessageField").GetComponent<UnityEngine.UI.Text>();
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        

        foreach (GameObject go in allObjects)
        {
            if (go.GetComponent<GameSystemManager>() != null)
            {
                gameSystemManager = go;
            }
        }

        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.S))
        //    SendMessageToHost("Hello from client");

        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    //Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }

    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "192.168.2.38", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }

    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }

    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);

        string[] csv = msg.Split(',');

        int signifier = int.Parse (csv[0]);

        if (signifier == ServerToClientSignifiers.AccountCreationComplete)
        {
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.MainMenu);
        }
        else if (signifier == ServerToClientSignifiers.LoginComplete)
        {
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.MainMenu);
        }

        else if (signifier == ServerToClientSignifiers.GameStart)
        {
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
            moves = new int[18];
            movesI = 0;
        }

        else if (signifier == ServerToClientSignifiers.OpponentPlay)
        {
            Debug.Log("Opponent Play!");
        }

        else if (signifier == ServerToClientSignifiers.ClientToClientMsgReceived)
        {
            Debug.Log("msgreceived");
            TextBox.text = TextBox.text + csv[1] + "\n";
        }

        else if (signifier == ServerToClientSignifiers.ClientMoveReceived)
        {
            gameSystemManager.GetComponent<GameSystemManager>().PlayerMoved(int.Parse(csv[1]), int.Parse(csv[2]));
            
            moves[movesI + 1] = int.Parse(csv[2]);
            movesI += 2;
            gameSystemManager.GetComponent<GameSystemManager>().CheckForWinner();
        }

        else if (signifier == ServerToClientSignifiers.WinnerTold)
        {
            gameSystemManager.GetComponent<GameSystemManager>().PlayerWon(int.Parse(csv[1]));
        }

        else if (signifier == ServerToClientSignifiers.NewGameRequest)
        {
            gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
        }
    }

    public bool IsConnected()
    {
        return isConnected;
    }
    

}

public static class ClientToServerSignifiers
{
    public const int CreateAccount = 1;

    public const int Login = 2;

    public const int JoinQueueForGameRoom = 3;

    public const int TicTacToeSomethingSomethingPlay = 4; //will be changed when working on assignment

    public const int ClientToClientMsgSent = 5;

    public const int ClientMoveSent = 6;

    public const int WinnerFound = 7;

    public const int NewGame = 8;

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

    public const int NewGameRequest = 10;
}
