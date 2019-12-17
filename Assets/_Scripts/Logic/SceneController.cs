using UnityEngine;
using Photon.Pun;
using System;

public class SceneController : MonoBehaviour
{

    public GameObject playerPrefab;

    void Start()
    {
        // Instatiate player object for all clients
        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,5f,0f), Quaternion.identity, 0);
    }
}
