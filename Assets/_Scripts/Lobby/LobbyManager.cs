using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    const string playerNamePrefKey = "PlayerName";

    private PlayerInfo player;

    [Header("Input fields")]
    public InputField inputField;
    public InputField gameName;

    [Header("Buttons")]
    public Button newGameButton;
    public Button findGameButton;
    public Button exitGameButton;
    

    [Header("Panels")]
    public GameObject namePanel;
    public GameObject menuPanel;
    public GameObject newGamePanel;


    public LobbyManager(){
        this.player = new PlayerInfo();
    }
    
    public void Start() {
        this.ActivePanel(namePanel.name);
    }

    public void ConnectToServer() {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateNewUser(){
        Debug.Log("starting to create user");
        this.player.SetPlayerName(this.inputField.text);

        PhotonNetwork.NickName = PlayerPrefs.GetString(playerNamePrefKey);
        this.ActivePanel(menuPanel.name);
    }

    public void CreateGame(){
        Debug.Log("create a game");
        string gameName = this.gameName.text.Trim();

        if(!string.IsNullOrEmpty(gameName)){
            PhotonNetwork.CreateRoom(gameName, new RoomOptions{MaxPlayers = 4});
            Debug.Log("game created");
            // change to the game
        }
        // cant create a game with null or nothing as name.
    }


    public void ActivePanel(string panel)
    {
        namePanel.SetActive(namePanel.name.Equals(panel));
        menuPanel.SetActive(menuPanel.name.Equals(panel));
        newGamePanel.SetActive(newGamePanel.name.Equals(panel));
    }

    // Menu selection functions


    public void newGame(){
        this.ActivePanel(newGamePanel.name);
    }

    public void FindGame(){
    }

    public void ExitGame(){}
    
}
