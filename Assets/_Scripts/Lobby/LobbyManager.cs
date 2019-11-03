using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("RoomPanel")]
    public GameObject roomPanel;
    public Dictionary<string, RoomInfo> roomList;

    [Header("CreateRoomPanel")]
    public GameObject createRoomPanel;
    
    public void Start() {
        roomPanel.SetActive(false);
    }

    public void ConnectToServer() {
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnRoomListUpdate(List<Photon.Realtime.RoomInfo> roomList)
    {
        roomList[0].
    }
    
}
