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
    private RoomController roomController;

    [Header("Input fields")]
    public InputField inputField;
    public InputField gameName;

    [Header("Buttons")]
    public Button newGameButton;
    public Button findGameButton;
    public Button exitGameButton;
    
    [Header("Prefabs")]
    public GameObject listObject;

    [Header("GameList Objects")]
    public GameObject listParent;
    private Dictionary<string, GameObject> roomListGameObjects;

    [Header("Panels")]
    public GameObject namePanel;
    public GameObject menuPanel;
    public GameObject newGamePanel;

    public GameObject allGamesPanel;

    
    public void Start() {
        this.ActivePanel(namePanel.name);
        this.player = new PlayerInfo();
        this.roomController = new RoomController();
        roomListGameObjects = new Dictionary<string, GameObject>();

    }

    public void ConnectToServer() {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateNewUser(){
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
        if(!PhotonNetwork.InLobby){
            PhotonNetwork.JoinLobby();
        }
        this.ActivePanel(allGamesPanel.name);

    }

    public void ExitGame(){}

    public void BackToMenu(){
        if(PhotonNetwork.InLobby){
            PhotonNetwork.LeaveLobby();
        }
        this.ActivePanel(menuPanel.name);
    }



    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        ClearRoomList();

        roomController.UpdateRooms(roomList);

        Dictionary<string, RoomInfo> cacheRoomList = roomController.GetRooms();

        foreach(RoomInfo room in cacheRoomList.Values)
        {
            GameObject roomListEntryGameObject = Instantiate(listObject);

            roomListEntryGameObject.transform.SetParent(listParent.transform);
            roomListEntryGameObject.transform.localScale = Vector3.one;

            roomListEntryGameObject.transform.Find("nameValue").GetComponent<Text>().text = room.Name;
            roomListEntryGameObject.transform.Find("playerValue").GetComponent<Text>().text = room.PlayerCount + "/" + room.MaxPlayers;
            roomListEntryGameObject.transform.Find("openValue").GetComponent<Text>().text = room.IsOpen ? "Open" :  "Closed";
            roomListEntryGameObject.GetComponent<Clickable>().OnClick += () => {};
            roomListGameObjects.Add(room.Name, roomListEntryGameObject);
        }
    }



    private void ClearRoomList()
    {
        foreach(GameObject obj in roomListGameObjects.Values)
        {
            Destroy(obj);
        }
        roomListGameObjects.Clear();
    }
    
}
