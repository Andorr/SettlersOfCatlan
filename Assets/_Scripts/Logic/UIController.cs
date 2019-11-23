using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Player Elements")]
    public GameObject actionPanel;
    public GameObject playerOnePanel;
    public GameObject playerTwoPanel;
    public GameObject playerThreePanel;
    public GameObject playerFourPanel;

    private Dictionary<string, GameObject> playerPanels = new Dictionary<string, GameObject>(); // (playerId, playerPanel)

    public bool AddPlayer(Player player) {
        if (playerPanels.ContainsKey(player.id)) {
            return false;
        }

        // Calculate player number
        int playerNumber = playerPanels.Count + 1;
        if(playerNumber > 4) {
            return false;
        }

        GameObject panel = null;
        if(playerNumber == 1) {
            panel = playerOnePanel;
        } else if(playerNumber == 2) {
            panel = playerTwoPanel;
        } else if (playerNumber == 3) {
            panel = playerThreePanel;
        } else {
            panel = playerFourPanel;
        }
        playerPanels.Add(player.id, panel);

        // Set player color
        panel.transform.GetChild(0).GetComponent<Image>().color = player.GetColor();
        panel.transform.GetChild(1).GetComponent<Image>().color = player.GetColor();
        

        return true;
    }

    public void UpdatePlayerUI(Player player) {
        var hasPlayer = playerPanels.TryGetValue(player.id, out var panel);
        if(!hasPlayer) {
            return;
        }

        // Update resource count
        var resourceController = panel.GetComponentInChildren<UIResourceController>();
        resourceController.UpdateResourceCount(player);

        // Update player victory points count
        panel.transform.GetChild(0).GetComponentInChildren<Text>().text = player.victoryPoints.ToString();

    }


    public void EnableActionPanel(bool enable, UnityAction roadAction = null, UnityAction houseAction = null, UnityAction cityAction = null)
    {
        actionPanel.SetActive(enable);
        var btns = actionPanel.GetComponentsInChildren<Button>();

        if(roadAction != null)
        {
            btns[0].onClick.AddListener(roadAction);
        }

        if(houseAction != null)
        {
            btns[1].onClick.AddListener(houseAction);
        }

        if(cityAction != null)
        {
            btns[2].onClick.AddListener(cityAction);
        }
    }

    public void EnableActionButtons(bool pathButton, bool houseButton, bool cityButton)
    {
        var btns = actionPanel.GetComponentsInChildren<Button>();
        btns[0].interactable = pathButton;
        btns[1].interactable = houseButton;
        btns[2].interactable = cityButton;
    }
}
