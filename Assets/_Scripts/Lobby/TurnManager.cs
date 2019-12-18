using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;


public interface ITurnCallback {
    void NewTurn(string newPlayer);
}


public interface ITurnManager {
    void StartTurnBased();
    void EndTurn();
}

public class TurnManager : MonoBehaviour, ITurnManager
{

    private PhotonView photonView;

    private string currentPlayerTurn = ""; // Saved for every client
    private Player currentPlayer; // Saved for master client only

    

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient && photonView.IsMine) {
            this.StartTurnBased();
        }
    }

    public void StartTurnBased() {
        if (currentPlayerTurn != "") {
            Debug.Log("Turn based already started");
        } else {
            Debug.Log("Starting turn based");
            this.photonView.RPC("RPCMasterTurnInit", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void RPCMasterTurnInit() {
        if (PhotonNetwork.IsMasterClient && currentPlayerTurn == "") {
            var firstPlayer = PhotonNetwork.PlayerList[Random.Range(0, PhotonNetwork.PlayerList.Length)];
            currentPlayer = firstPlayer;
            this.photonView.RPC("RPCAssignTurnToPlayer", RpcTarget.All, firstPlayer.UserId);
        }
    }

    [PunRPC]
    void RPCAssignTurnToPlayer(string playerID) {
        this.currentPlayerTurn = playerID;
        foreach (var listener in FindObjectsOfType<MonoBehaviour>().OfType<ITurnCallback>()) {
            listener.NewTurn(playerID);
        }
        Debug.Log($"[TurnManager]: It is now {playerID}'s turn!");
    }


    public void EndTurn() {
        Debug.Log($"Current Player Turn: {currentPlayerTurn} ----- LocalPlayer User Id: {PhotonNetwork.LocalPlayer.UserId}");
        if (currentPlayerTurn.Equals(PhotonNetwork.LocalPlayer.UserId)) {
            this.photonView.RPC("RPCMasterCallEndTurn", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.UserId);
        } else {
            Debug.Log("Not your turn: " + PhotonNetwork.LocalPlayer.UserId);
        }
    }

    [PunRPC]
    void RPCMasterCallEndTurn(string playerID) {
        if (PhotonNetwork.IsMasterClient) {
            if (playerID == currentPlayer.UserId) {
                var nextPlayer = currentPlayer.GetNext();
                if(nextPlayer == null) {
                    nextPlayer = currentPlayer;
                }
                this.currentPlayer = nextPlayer;
                this.photonView.RPC("RPCAssignTurnToPlayer", RpcTarget.All, nextPlayer.UserId);
            }
        }
    }
}
