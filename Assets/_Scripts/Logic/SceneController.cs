using UnityEngine;
using Photon.Pun;
using System;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public GameObject playerPrefab;

    void Awake() {
        if(!PhotonPeer.RegisterType(typeof(ResourceStorage), (byte)'L', ResourceStorage.Serialize, ResourceStorage.Deserialize)) {
            throw new Exception("Was not able to register ResourceStorage to Photon");
        }
    }

    void Start()
    {
        // Instatiate player object for all clients
        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,5f,0f), Quaternion.identity, 0);
    }

    public static void LoadLobbyScene() {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Lobby");
    }
}
