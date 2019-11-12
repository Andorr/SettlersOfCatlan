using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;


public class LobbyManager : MonoBehaviourPunCallbacks
{
    const int GAMESCENE = 1;
    const string playerNamePrefKey = "PlayerName";

    private PlayerInfo player;
    private RoomController roomController;
    
    private Dictionary<int, GameObject> players;
    private Dictionary<string, GameObject> roomListGameObjects;

    [Header("Input fields")]
    public InputField inputField;
    public InputField gameName;

    [Header("Buttons")]
    public Button newGameButton;
    public Button findGameButton;
    public Button exitGameButton;
    public Button startGameButton;
    
    [Header("Prefabs")]
    public GameObject listObject;
    public GameObject playerObject;

    [Header("GameList Objects")]
    public GameObject listParent;
    public GameObject listPlayerParent;


    [Header("Panels")]
    public GameObject namePanel;
    public GameObject menuPanel;
    public GameObject newGamePanel;
    public GameObject detailGamePanel;
    public GameObject allGamesPanel;

    
    public void Start() {
        this.ActivePanel(namePanel.name);
        this.player = new PlayerInfo();
        this.roomController = new RoomController();
        roomListGameObjects = new Dictionary<string, GameObject>();
        ConnectToServer();
        PhotonNetwork.AutomaticallySyncScene = true;
        players = new Dictionary<int, GameObject>();
    }

    public void ConnectToServer() {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateNewUser(){
        this.player.SetPlayerName(this.inputField.text);

        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString(playerNamePrefKey);
        this.ActivePanel(menuPanel.name);
    }

    public void CreateGame(){
        string gameName = this.gameName.text.Trim();

        if(!string.IsNullOrEmpty(gameName)){
            PhotonNetwork.CreateRoom(gameName, new RoomOptions{MaxPlayers = 4});
        }
    }

    public void ActivePanel(string panel)
    {
        namePanel.SetActive(namePanel.name.Equals(panel));
        menuPanel.SetActive(menuPanel.name.Equals(panel));
        newGamePanel.SetActive(newGamePanel.name.Equals(panel));
        detailGamePanel.SetActive(detailGamePanel.name.Equals(panel));
        allGamesPanel.SetActive(allGamesPanel.name.Equals(panel));
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

    public void ExitGame(){
        Application.Quit();
    }

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
            roomListEntryGameObject.GetComponent<Clickable>().OnClick += () => {
                PhotonNetwork.JoinRoom(room.Name);
            };
            roomListGameObjects.Add(room.Name, roomListEntryGameObject);
        }
    }
    
    // Called when joinroom or createRoom is called. 
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " joined to " + PhotonNetwork.CurrentRoom.Name);
        ActivePanel(detailGamePanel.name);

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.gameObject.SetActive(true);
        } else {
            startGameButton.gameObject.SetActive(false);
        }
        foreach(Photon.Realtime.Player player in PhotonNetwork.PlayerList){
            AddPlayer(player);
        }
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer){
        Destroy(players[otherPlayer.ActorNumber].gameObject);
        players.Remove(otherPlayer.ActorNumber);

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.gameObject.SetActive(true);
        }
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer){
        AddPlayer(newPlayer);
    }

    public override void OnLeftLobby()
    {
        ClearRoomList();
        roomController.Clear();
    }

    public override void OnLeftRoom()
    {
        ActivePanel(menuPanel.name);

        foreach(GameObject player in players.Values)
        {
            Destroy(player);
        }
        players.Clear();
        players = null;
    }

    public void StartGame(){
        // Here to player starts the game! by clicking the button
        PhotonNetwork.LoadLevel(GAMESCENE);
    }

    private void ClearRoomList(){
        
        foreach(GameObject obj in roomListGameObjects.Values)
        {
            Destroy(obj);
        }
        roomListGameObjects.Clear();
    }


    private void AddPlayer(Photon.Realtime.Player player){
        GameObject playerListEntryObject = Instantiate(playerObject);
        playerListEntryObject.transform.SetParent(listPlayerParent.transform);
        playerListEntryObject.transform.localScale = Vector3.one;

        playerListEntryObject.transform.GetComponentInChildren<Text>().text  = player.NickName;

        players.Add(player.ActorNumber, playerListEntryObject);
    }
}
