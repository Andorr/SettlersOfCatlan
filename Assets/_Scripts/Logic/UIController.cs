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
    public Image eventImage;
    public GameObject sideActionPanel;
    public GameObject cardView;
    public WinPanelController winPanel;
    public ResourceItemController resourceItemController;
    public TradingViewController tradingViewController;
    public TradeRequestViewController tradeRequestViewController;
    public CardItemController cardItemController;

    [Header("Player Elements")]
    public GameObject actionPanel;
    public GameObject playerOnePanel;
    public GameObject playerTwoPanel;
    public GameObject playerThreePanel;
    public GameObject playerFourPanel;

    [Header("Sprites")]
    public Sprite cardVP;
    public Sprite cardKnight;
    public Sprite knight;

    private Dictionary<string, GameObject> playerPanels = new Dictionary<string, GameObject>(); // (playerId, playerPanel)

    public void Awake() {
        InitializeFonts();
        controller = GetComponent<GameController>();
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
    
        
        var cardController = panel.GetComponentInChildren<CardViewController>();

        if(cardController != null) {
            cardController.UpdateCardCount(player);
        }

        PlayerController playerCont = controller.GetLocalPlayer();

        if(playerCont != null){
            cardItemController.UpdateCards(playerCont);
        }
       

        // Update player victory points count
        panel.transform.GetChild(0).GetComponentInChildren<Text>().text = player.victoryPoints.ToString();
    }



    public void ShowPlayerTurn(string playerId) {
        foreach(var player in playerPanels.Keys) {
            playerPanels[player].transform.GetChild(0).GetComponent<Image>().enabled = player.Equals(playerId);
        }
    }

    public void EnableActionPanel(bool enable, UnityAction roadAction = null, UnityAction houseAction = null, UnityAction cityAction = null, UnityAction cardAction = null)
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
        if(cardAction != null)
        {
            btns[3].onClick.RemoveAllListeners();
            btns[3].onClick.AddListener(cardAction);

        }
    }

    public void EnableActionButtons(bool pathButton, bool houseButton, bool cityButton, bool cardButton)
    {
        var btns = actionPanel.GetComponentsInChildren<Button>();
        btns[0].interactable = pathButton;
        btns[1].interactable = houseButton;
        btns[2].interactable = cityButton;
        btns[3].interactable = cardButton;
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

    public void DisplayEventImage(Sprite image, float duration) {
        eventImage.sprite = image;
        eventImage.GetComponent<GraphicFade>().FadeInAndOut(1f, duration);
    }

    public void DisplayGainedResource(string playerName, ResourceStorage storage) {
        resourceItemController.ShowResources(playerName, storage);
    }

    public void DisplayUsedCard(string displayName, Card card, string customTitle = null) {
        string title = null;
        Sprite image = null;

        switch(card.cardType) {
            case CardType.VP: {
                title = $"{displayName} gained one victory point!";
                image = cardVP;
                break;
            }
            case CardType.Thief: {
                title = $"{displayName} gained a theif!";
                image = cardKnight;
                break;
            }
        }

        if(customTitle != null) {
            title = customTitle;
        }

        if(title == null || image == null) {
            return;
        }
                        
        DisplayEventText(title, 3f);
        DisplayEventImage(image, 3f);
    }

    public void EnableWinPanel(bool enable, Player winner) {
        winPanel.EnableWinPanel(enable, winner);
    }

    public void EnableSideActionPanel(bool enable) {
        sideActionPanel.SetActive(enable);
    }

    public void EnableCardView(bool enable){
        cardView.SetActive(enable);
    }

    public void EnableTrading() {
        var gameController = GetComponent<GameController>();
        var localPlayer = gameController.GetPlayers(out var otherPlayers);
        tradingViewController.canCancelWithESC = true;
        tradingViewController.ShowTradingPanel(localPlayer, otherPlayers, gameController.ExchangeResources, gameController.SendTradeRequest, gameController.SendTradeRequestCancellation);
    }

    public void EnablePlayerPick(TradingViewController.OnPlayerSelect callback) {
        var gameController = GetComponent<GameController>();
        var localplayer = gameController.GetPlayers(out var otherPlayers);
        tradingViewController.canCancelWithESC = false;
        tradingViewController.EnablePlayerSelect(otherPlayers, callback);
    }

    public void DisableTrading() {
        tradingViewController.Disable();
        tradingViewController.canCancelWithESC = true;
    }

    public void ShowTradeRequest(Player playerToTradeWith, ResourceStorage from, ResourceStorage to)
    {
        var gameController = GetComponent<GameController>();
        tradingViewController.canCancelWithESC = false;
        tradeRequestViewController.Initialize(playerToTradeWith, from, to, gameController.SendTradeRequestAnswer);
    }

    public void EnableCardItems(){
        var gameController = GetComponent<GameController>();
        var localPlayer = gameController.GetPlayers(out var otherPlayers);
        cardItemController.showCardItems();
    }

    public bool IsUIPanelsOpen() {
        return tradingViewController.gameObject.activeSelf || tradeRequestViewController.gameObject.activeSelf || cardItemController.gameObject.activeSelf;
    }

    private void InitializeFonts() {
        foreach(Text t in Component.FindObjectsOfType<Text>()) {
            t.font = font;
        }
    }

}
