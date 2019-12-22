using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private GameController controller;

    [Header("UI")]
    public Font font;
    public Button endTurnButton;
    public Text eventText;
    public GameObject sideActionPanel;
    public WinPanelController winPanel;
    public ResourceItemController resourceItemController;
    public TradingViewController tradingViewController;

    [Header("Player Elements")]
    public GameObject actionPanel;
    public GameObject playerOnePanel;
    public GameObject playerTwoPanel;
    public GameObject playerThreePanel;
    public GameObject playerFourPanel;

    private Dictionary<string, GameObject> playerPanels = new Dictionary<string, GameObject>(); // (playerId, playerPanel)

    public void Awake() {
        InitializeFonts();
        playerOnePanel.SetActive(false);
        playerTwoPanel.SetActive(false);
        playerThreePanel.SetActive(false);
        playerFourPanel.SetActive(false);
    }

    public bool AddPlayer(Player player, string nickName) {
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
        panel.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = player.GetColor();
        var namePanel = panel.transform.GetChild(1);
        namePanel.GetComponent<Image>().color = player.GetColor();
        namePanel.GetComponentInChildren<Text>().text = nickName;

        
        panel.SetActive(true);

        return true;
    }

    public void UpdatePlayerUI(Player player) {
        var hasPlayer = playerPanels.TryGetValue(player.id, out var panel);
        if(!hasPlayer) {
            return;
        }

        // Update resource count
        var resourceController = panel.GetComponentInChildren<ResourceViewController>();
        resourceController.UpdateResourceCount(player);

        // Update player victory points count
        panel.transform.GetChild(0).GetComponentInChildren<Text>().text = player.victoryPoints.ToString();
    }

    public void ShowPlayerTurn(string playerId) {
        foreach(var player in playerPanels.Keys) {
            playerPanels[player].transform.GetChild(0).GetComponent<Image>().enabled = player.Equals(playerId);
        }
    }

    public void EnableActionPanel(bool enable, UnityAction roadAction = null, UnityAction houseAction = null, UnityAction cityAction = null)
    {
        actionPanel.SetActive(enable);
        var btns = actionPanel.GetComponentsInChildren<Button>();

        if(roadAction != null)
        {
            btns[0].onClick.RemoveAllListeners();
            btns[0].onClick.AddListener(roadAction);
        }

        if(houseAction != null)
        {
            btns[1].onClick.RemoveAllListeners();
            btns[1].onClick.AddListener(houseAction);
        }

        if(cityAction != null)
        {
            btns[2].onClick.RemoveAllListeners();
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

    public void EnableEndTurnButton(bool enable, UnityAction action)
    {
        endTurnButton.gameObject.SetActive(enable);
        endTurnButton.onClick.RemoveAllListeners();
        endTurnButton.onClick.AddListener(action);
    }

    public void DisplayEventText(string title, float duration = 5) {
        eventText.text = title;
        eventText.GetComponent<GraphicFade>().FadeInAndOut(1f, duration);
    }

    public void DisplayGainedResource(string playerName, ResourceStorage storage) {
        resourceItemController.ShowResources(playerName, storage);
    }

    public void EnableWinPanel(bool enable, Player winner) {
        winPanel.EnableWinPanel(enable, winner);
    }

    public void EnableSideActionPanel(bool enable) {
        sideActionPanel.SetActive(enable);
    }

    public void EnableTrading() {
        var gameController = GetComponent<GameController>();
        var localPlayer = gameController.GetPlayers(out var otherPlayers);
        tradingViewController.ShowTradingPanel(localPlayer, otherPlayers, gameController.ExchangeResources);
    }

    private void InitializeFonts() {
        foreach(Text t in Component.FindObjectsOfType<Text>()) {
            t.font = font;
        }
    }
}
