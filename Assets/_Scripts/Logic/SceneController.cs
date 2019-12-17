using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SceneController : MonoBehaviour
{

    public GameObject playerPrefab;


    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f,5f,0f), Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
