using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    GameObject submitButton, userNameInput, passwordInput, createToggle, loginToggle, textNameInfo, textPasswordInfo, ticTacToeSquareButton, networkedClient, GGWP, GLHF, CUCK , GameMap, GameSceneUI;
    
    GameObject joinGameRoomButton, GameOver, SendText, ChatField, ReplayButton, RestartButton;

    GameObject a1, a2, a3, b1, b2, b3, c1, c2, c3;
    GameObject cA1, cA2, cA3, cB1, cB2, cB3, cC1, cC2, cC3;
    GameObject xA1, xA2, xA3, xB1, xB2, xB3, xC1, xC2, xC3;

    void Start()
    {

        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach(GameObject go in allObjects)
        {

            if (go.name == "UserNameInputField")
                userNameInput = go;
            else if (go.name == "PasswordInputField")
                passwordInput = go;
            else if (go.name == "SubmitButton")
                submitButton = go;
            else if (go.name == "LoginToggle")
                loginToggle = go;
            else if (go.name == "CreateToggle")
                createToggle = go;
            else if (go.name == "NetworkedClient")
                networkedClient = go;
            else if (go.name == "JoinGameRoomButton")
                joinGameRoomButton = go;
            else if (go.name == "UserNameText")
                textNameInfo = go;
            else if (go.name == "PasswordText")
                textPasswordInfo = go;
            else if (go.name == "TicTacToeSquareButton")
                ticTacToeSquareButton = go;
            else if (go.name == "GGWP")
                GGWP = go;
            else if (go.name == "GLHF")
                GLHF = go;
            else if (go.name == "CUCK")
                CUCK = go;
            else if (go.name == "GameMap")
                GameMap = go;
            else if (go.name == "GameSceneUI")
                GameSceneUI = go;
            else if (go.name == "a1")
                a1 = go;
            else if (go.name == "a2")
                a2 = go;
            else if (go.name == "a3")
                a3 = go;
            else if (go.name == "b1")
                b1 = go;
            else if (go.name == "b2")
                b2 = go;
            else if (go.name == "b3")
                b3 = go;
            else if (go.name == "c1")
                c1 = go;
            else if (go.name == "c2")
                c2 = go;
            else if (go.name == "c3")
                c3 = go;
            else if (go.name == "cA1")
                cA1 = go;
            else if (go.name == "cA2")
                cA2 = go;
            else if (go.name == "cA3")
                cA3 = go;
            else if (go.name == "cB1")
                cB1 = go;
            else if (go.name == "cB2")
                cB2 = go;
            else if (go.name == "cB3")
                cB3 = go;
            else if (go.name == "cC1")
                cC1 = go;
            else if (go.name == "cC2")
                cC2 = go;
            else if (go.name == "cC3")
                cC3 = go;
            else if (go.name == "xA1")
                xA1 = go;
            else if (go.name == "xA2")
                xA2 = go;
            else if (go.name == "xA3")
                xA3 = go;
            else if (go.name == "xB1")
                xB1 = go;
            else if (go.name == "xB2")
                xB2 = go;
            else if (go.name == "xB3")
                xB3 = go;
            else if (go.name == "xC1")
                xC1 = go;
            else if (go.name == "xC2")
                xC2 = go;
            else if (go.name == "xC3")
                xC3 = go;
            else if (go.name == "Gameover")
                GameOver = go;
            else if (go.name == "SendButton")
                SendText = go;
            else if (go.name == "ChatField")
                ChatField = go;
            else if (go.name == "Replay")
                ReplayButton = go;
            else if (go.name == "NewGame")
                RestartButton = go;
        }

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        loginToggle.GetComponent<Toggle>().onValueChanged.AddListener(LoginToggleChanged);
        createToggle.GetComponent<Toggle>().onValueChanged.AddListener(CreateToggleChanged);

        joinGameRoomButton.GetComponent<Button>().onClick.AddListener(JoinGameRoomButtonPressed);

        ticTacToeSquareButton.GetComponent<Button>().onClick.AddListener(TicTacToeSquareButtonPressed);

        GGWP.GetComponent<Button>().onClick.AddListener(GGWPPressed);
        GLHF.GetComponent<Button>().onClick.AddListener(GLHFPressed);
        CUCK.GetComponent<Button>().onClick.AddListener(CUCKPressed);
        SendText.GetComponent<Button>().onClick.AddListener(SendPressed);
        ReplayButton.GetComponent<Button>().onClick.AddListener(ReplayPressed);
        RestartButton.GetComponent<Button>().onClick.AddListener(RestartPressed);

        a1.GetComponent<Button>().onClick.AddListener(a1Pressed);
        a2.GetComponent<Button>().onClick.AddListener(a2Pressed);
        a3.GetComponent<Button>().onClick.AddListener(a3Pressed);
        b1.GetComponent<Button>().onClick.AddListener(b1Pressed);
        b2.GetComponent<Button>().onClick.AddListener(b2Pressed);
        b3.GetComponent<Button>().onClick.AddListener(b3Pressed);
        c1.GetComponent<Button>().onClick.AddListener(c1Pressed);
        c2.GetComponent<Button>().onClick.AddListener(c2Pressed);
        c3.GetComponent<Button>().onClick.AddListener(c3Pressed);

        ChangeState(GameStates.LoginMenu);

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void SubmitButtonPressed()
    {

        //We want to send login stuff to the server!!!!

        string n = userNameInput.GetComponent<InputField>().text;
        string p = passwordInput.GetComponent<InputField>().text;
        
        string msg;
        
        if(createToggle.GetComponent<Toggle>().isOn)
            msg= ClientToServerSignifiers.CreateAccount + "," + n + "," + p;
        else
            msg = ClientToServerSignifiers.Login + "," + n + "," + p;

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        Debug.Log(msg);
    }

    public void LoginToggleChanged(bool newValue)
    {
        //Debug.Log("Login Toggle working");
        createToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }

    public void CreateToggleChanged(bool newValue)
    {
        //Debug.Log("Create Toggle working");
        loginToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!newValue);
    }


    public void ChangeState(int newState)
    {
        joinGameRoomButton.SetActive(false);
        submitButton.SetActive(false);
        userNameInput.SetActive(false);
        passwordInput.SetActive(false);
        createToggle.SetActive(false);
        loginToggle.SetActive(false);

        textNameInfo.SetActive(false);
        textPasswordInfo.SetActive(false);
        joinGameRoomButton.SetActive(false);
        ticTacToeSquareButton.SetActive(false);
        GameOver.SetActive(false);
        GameSceneUI.SetActive(false);
        GameMap.SetActive(false);

        if (newState == GameStates.LoginMenu)
        {
            submitButton.SetActive(true);
            userNameInput.SetActive(true);
            passwordInput.SetActive(true);
            createToggle.SetActive(true);
            loginToggle.SetActive(true);
            textNameInfo.SetActive(true);
            textPasswordInfo.SetActive(true);
            
            
        }
        else if (newState == GameStates.MainMenu)
        {
            joinGameRoomButton.SetActive(true);
        }

        else if (newState == GameStates.WaitingInQueueForOtherPlayer)
        {
            joinGameRoomButton.SetActive(true); //may change to be back button
        }

        else if (newState == GameStates.TicTacToe)
        {
            //set tictactoe game board UI stuffs to active
            
            GameSceneUI.SetActive(true);
            GameMap.SetActive(true);
            xA1.SetActive(false);
            cA1.SetActive(false);
            xA2.SetActive(false);
            cA2.SetActive(false);
            xA3.SetActive(false);
            cA3.SetActive(false);
            xB1.SetActive(false);
            cB1.SetActive(false);
            xB2.SetActive(false);
            cB2.SetActive(false);
            xB3.SetActive(false);
            cB3.SetActive(false);
            xC1.SetActive(false);
            cC1.SetActive(false);
            xC2.SetActive(false);
            cC2.SetActive(false);
            xC3.SetActive(false);
            cC3.SetActive(false);
            a1.SetActive(true);
            a2.SetActive(true);
            a3.SetActive(true);
            b1.SetActive(true);
            b2.SetActive(true);
            b3.SetActive(true);
            c1.SetActive(true);
            c2.SetActive(true);
            c3.SetActive(true);
        }
        else if (newState == GameStates.Replay)
        {
            GameSceneUI.SetActive(true);
            GameMap.SetActive(true);
            xA1.SetActive(false);
            cA1.SetActive(false);
            xA2.SetActive(false);
            cA2.SetActive(false);
            xA3.SetActive(false);
            cA3.SetActive(false);
            xB1.SetActive(false);
            cB1.SetActive(false);
            xB2.SetActive(false);
            cB2.SetActive(false);
            xB3.SetActive(false);
            cB3.SetActive(false);
            xC1.SetActive(false);
            cC1.SetActive(false);
            xC2.SetActive(false);
            cC2.SetActive(false);
            xC3.SetActive(false);
            cC3.SetActive(false);
            a1.SetActive(false);
            a2.SetActive(false);
            a3.SetActive(false);
            b1.SetActive(false);
            b2.SetActive(false);
            b3.SetActive(false);
            c1.SetActive(false);
            c2.SetActive(false);
            c3.SetActive(false);
            GameOver.SetActive(true);
        }
    }


    public void JoinGameRoomButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.JoinQueueForGameRoom + "");
        ChangeState(GameStates.WaitingInQueueForOtherPlayer);
    }

    public void TicTacToeSquareButtonPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.TicTacToeSomethingSomethingPlay + "");
    }

    public void GGWPPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ClientToClientMsgSent + ",GGWP");
    }

    public void GLHFPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ClientToClientMsgSent + ",GLHF");
    }

    public void CUCKPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ClientToClientMsgSent + ",CUCK" );
    }
    
    public void SendPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ClientToClientMsgSent + "," + ChatField.GetComponent<InputField>().text);
        ChatField.GetComponent<InputField>().text = "";
    }

    public void ReplayPressed()
    {
        ChangeState(GameStates.Replay);
        int n = (networkedClient.GetComponent<NetworkedClient>().moves.Length);
        for (int i = 0; i < n; i+=2)
        {
            StartCoroutine(ReplayMove(i));
        }
    }
    public void RestartPressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.NewGame + "");

    }


    public void a1Pressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ClientMoveSent + ",1");
    }
    public void a2Pressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ClientMoveSent + ",2");
     }
    public void a3Pressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ClientMoveSent + ",3");
    }
    public void b1Pressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ClientMoveSent + ",4");
    }
    public void b2Pressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ClientMoveSent + ",5");
    }
    public void b3Pressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ClientMoveSent + ",6");
    }
    public void c1Pressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ClientMoveSent + ",7");
    }
    public void c2Pressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ClientMoveSent + ",8");
    }
    public void c3Pressed()
    {
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.ClientMoveSent + ",9");
        
    }

    IEnumerator ReplayMove(int i)
    {
        yield return new WaitForSeconds(i/2);        
        PlayerMoved(networkedClient.GetComponent<NetworkedClient>().moves[i], networkedClient.GetComponent<NetworkedClient>().moves[i + 1]);
    }

    public void PlayerMoved(int n, int p)
    {
        
        switch (n)
        {
            case 1:
                if (p == 1)
                {
                    xA1.SetActive(true);
                    a1.SetActive(false);
                }
                else
                { 
                    cA1.SetActive(true);
                    a1.SetActive(false);
                }
                break;

            case 2:
                if (p == 1)
                {
                    xA2.SetActive(true);
                    a2.SetActive(false);
                }
                else
                {
                    cA2.SetActive(true);
                    a2.SetActive(false);
                }
                break;

            case 3:
                if (p == 1)
                {
                    xA3.SetActive(true);
                    a3.SetActive(false);
                }
                else
                {
                    cA3.SetActive(true);
                    a3.SetActive(false);
                }
                break;

            case 4:
                if (p == 1)
                {
                    xB1.SetActive(true);
                    b1.SetActive(false);
                }
                else
                {
                    cB1.SetActive(true);
                    b1.SetActive(false);
                }
                break;

            case 5:
                if (p == 1)
                {
                    xB2.SetActive(true);
                    b2.SetActive(false);
                }
                else
                {
                    cB2.SetActive(true);
                    b2.SetActive(false);
                }
                break;

            case 6:
                if (p == 1)
                {
                    xB3.SetActive(true);
                    b3.SetActive(false);
                }
                else
                {
                    cB3.SetActive(true);
                    b3.SetActive(false);
                }
                break;

            case 7:
                if (p == 1)
                {
                    xC1.SetActive(true);
                    c1.SetActive(false);
                }
                else
                {
                    cC1.SetActive(true);
                    c1.SetActive(false);
                }
                break;

            case 8:
                if (p == 1)
                {
                    xC2.SetActive(true);
                    c2.SetActive(false);
                }
                else
                {
                    cC2.SetActive(true);
                    c2.SetActive(false);
                }
                break;

            case 9:
                if (p == 1)
                {
                    xC3.SetActive(true);
                    c3.SetActive(false);
                }
                else
                {
                    cC3.SetActive(true);
                    c3.SetActive(false);
                }
                break;
        }
    }
    public void PlayerWon(int n)
    {
        GameOver.SetActive(true);
        if (n == 1)
        {
            GameOver.GetComponent<RectTransform>().Find("Winner").GetComponent<UnityEngine.UI.Text>().text = "Cross' Victory!";
        }
        if (n == 2)
        {
            GameOver.GetComponent<RectTransform>().Find("Winner").GetComponent<UnityEngine.UI.Text>().text = "Circle's Victory!";
        }
        if (n == 3)
        {
            GameOver.GetComponent<RectTransform>().Find("Winner").GetComponent<UnityEngine.UI.Text>().text = "A tie... Boring";
        }
    }

    public void CheckForWinner()
    {
        if (xA1.activeSelf && xA2.activeSelf && xA3.activeSelf) 
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",1"); 
        if (xB1.activeSelf && xB2.activeSelf && xB3.activeSelf)
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",1");
        if (xC1.activeSelf && xC2.activeSelf && xC3.activeSelf)
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",1");
        if (xA1.activeSelf && xB1.activeSelf && xC1.activeSelf)
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",1");
        if (xA2.activeSelf && xB2.activeSelf && xC2.activeSelf)
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",1");
        if (xA3.activeSelf && xB3.activeSelf && xC3.activeSelf)
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",1");
        if (xA1.activeSelf && xB2.activeSelf && xC3.activeSelf)
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",1");
        if (xA3.activeSelf && xB2.activeSelf && xC1.activeSelf)
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",1");

        if (cA1.activeSelf && cA2.activeSelf && cA3.activeSelf)
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",2");
        if (cB1.activeSelf && cB2.activeSelf && cB3.activeSelf)                                                        
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",2");
        if (cC1.activeSelf && cC2.activeSelf && cC3.activeSelf)                                                        
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",2");
        if (cA1.activeSelf && cB1.activeSelf && cC1.activeSelf)                                                        
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",2");
        if (cA2.activeSelf && cB2.activeSelf && cC2.activeSelf)                                                        
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",2");
        if (cA3.activeSelf && cB3.activeSelf && cC3.activeSelf)                                                        
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",2");
        if (cA1.activeSelf && cB2.activeSelf && cC3.activeSelf)                                                        
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",2");
        if (cA3.activeSelf && cB2.activeSelf && cC1.activeSelf)                                                        
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(ClientToServerSignifiers.WinnerFound + ",2");

    }

    
}


static public class GameStates
{
    public const int LoginMenu = 1;
    public const int MainMenu = 2;
    public const int WaitingInQueueForOtherPlayer = 3;
    public const int TicTacToe = 4;
    public const int Replay = 5;
}
