using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    GameObject submitButton, userNameInput, passwordInput, createToggle, loginToggle, textNameInfo, textPasswordInfo, ticTacToeSquareButton, networkedClient;

    GameObject joinGameRoomButton;


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

        }

        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        loginToggle.GetComponent<Toggle>().onValueChanged.AddListener(LoginToggleChanged);
        createToggle.GetComponent<Toggle>().onValueChanged.AddListener(CreateToggleChanged);

        joinGameRoomButton.GetComponent<Button>().onClick.AddListener(JoinGameRoomButtonPressed);

        ticTacToeSquareButton.GetComponent<Button>().onClick.AddListener(TicTacToeSquareButtonPressed);

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
            ticTacToeSquareButton.SetActive(true);
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

}


static public class GameStates
{
    public const int LoginMenu = 1;
    public const int MainMenu = 2;
    public const int WaitingInQueueForOtherPlayer = 3;
    public const int TicTacToe = 4;
}
