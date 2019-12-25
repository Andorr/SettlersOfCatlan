using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using static ExchangeViewController;

public class TradingViewController : MonoBehaviour
{
    private delegate void OnPlayerSelect(Player player);
    private OnPlayerSelect playerSelectHandler;
    private ExchangeHandler onExchange;

    private Player localPlayer;
    private Player[] playersToTradeWith;

    [Header("Prefabs")]
    public GameObject playerCardPrefab;

    [Header("Trading Object")]
    public GameObject playerSelectPanel;
    public GameObject tradePanel;
    public GameObject exchangePanel;
    public GameObject playerTradePanel;

    public void ShowTradingPanel(Player localPlayer, Player[] playersToTradeWith, ExchangeHandler exchangeHandler) {
        this.localPlayer = localPlayer;
        this.playersToTradeWith = playersToTradeWith;
        onExchange = exchangeHandler;

        // Select one player to trade with
        gameObject.SetActive(true);
        tradePanel.SetActive(true);
        EnableExchangePanel();
    }

    private void EnablePlayerSelect(Player[] players) {
        playerSelectPanel.SetActive(true);

        for(int i = playerSelectPanel.transform.childCount - 1; i >= 0; i--) {
            Destroy(playerSelectPanel.transform.GetChild(i));
        }
        foreach(Player p in players) {
            GameObject obj = GameObject.Instantiate(playerCardPrefab, Vector3.zero, Quaternion.identity);
            obj.transform.SetParent(playerSelectPanel.transform.GetChild(1));
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => {
                playerSelectHandler(p);
            });
            obj.GetComponent<EventTrigger>().triggers.Add(entry);
        }
    }

    private void EnablePlayerTrade(Player currentPlayer, Player[] playersToTradeWith) {
        tradePanel.SetActive(true);
        

    }

    public void EnableExchangePanel() {
        exchangePanel.SetActive(true);
        playerTradePanel.SetActive(false);
        exchangePanel.GetComponent<ExchangeViewController>().Initialize(localPlayer.resources, (resourceA, resourceB) => {
            if(onExchange != null) {
                onExchange(resourceA, resourceB);
            }
        });
    }

    public void EnablePlayerTradePanel() {
        exchangePanel.SetActive(false);
        playerTradePanel.SetActive(true);
    }

    public void Disable() {
        playerTradePanel.SetActive(false);
        exchangePanel.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Update() {
        if(!gameObject.activeSelf) {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            Disable();
        }
    }
}
