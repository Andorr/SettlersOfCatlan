using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private PlayerInfo player;
    private MainMenu menu;
    public InputField inputField;
    public Button newGameButton;
    public Button findGameButton;
    public Button exitGameButton;
    
    public LobbyManager(){
        this.player = new PlayerInfo(inputField);
        this.menu = new MainMenu(newGameButton, findGameButton, exitGameButton);
        
    }



    [Header("Room Panel")]
    public GameObject namePanel;
    [Header("Menu Panel")]
    public GameObject menuPanel;

    [Header("CreateRoomPanel")]
    public GameObject createRoomPanel;
    
    public void Start() {
        this.ActivePanel(namePanel.name);
    }

    public void ConnectToServer() {
        PhotonNetwork.ConnectUsingSettings();
    }


    public void CreateNewUser(){
        PhotonNetwork.NickName = player.SetPlayerName();
        this.ActivePanel(menuPanel.name);
    }

    public void ActivePanel(string panel)
    {
        namePanel.SetActive(namePanel.name.Equals(panel));
        menuPanel.SetActive(menuPanel.name.Equals(panel));
    }
    
}
