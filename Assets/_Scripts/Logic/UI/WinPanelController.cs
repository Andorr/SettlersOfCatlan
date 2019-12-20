using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using UnityEngine.UI;

public class WinPanelController : MonoBehaviour
{
    [Header("Panels")]
    public Text winnerText;
    public Button leaveButton;

    public void EnableWinPanel(bool enable, Player winner) 
    {
        gameObject.SetActive(enable);

        if(winner == null) {
            return;
        }

        winnerText.text = winner.name + " won!";
    }
}
