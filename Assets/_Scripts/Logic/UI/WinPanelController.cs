using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using UnityEngine.UI;

public class WinPanelController : MonoBehaviour
{
    private AudioController audioController;

    [Header("Panels")]
    public Text winnerText;
    public Button leaveButton;

    [Header("AudioClips")]
    public AudioClip winClip;

    public void Start() {
        audioController = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>();
    }

    public void EnableWinPanel(bool enable, Player winner) 
    {
        gameObject.SetActive(enable);

        if(enable) {
            audioController.PlayClip(winClip);
            audioController.BackgroundMusic(false);
        }


        if(winner == null) {
            return;
        }
        winnerText.text = winner.name + " won!";
    }

    public void OnLeaveButtonClicked() {
        Photon.Pun.PhotonNetwork.LeaveRoom();
        Photon.Pun.PhotonNetwork.LeaveLobby();

        SceneController.LoadLobbyScene();
    }
}
