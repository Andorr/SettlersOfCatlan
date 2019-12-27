using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using static ExchangeViewController;
using static PlayerTradeViewController;
using UnityEngine.Events;

public class TradingViewController : MonoBehaviour
{
    private delegate void OnPlayerSelect(Player player);
    private OnPlayerSelect playerSelectHandler;
    private ExchangeHandler onExchange;
    private TradeHandler onTradeRequestSent;
    private TradeCancellation onTradeCancel;

    private Player localPlayer;
    private Player[] playersToTradeWith;

    public bool canCancelWithESC = true;

    [Header("Prefabs")]
    public GameObject playerCardPrefab;

    [Header("Trading Object")]
    public GameObject playerSelectPanel;
    public GameObject tradePanel;
    public GameObject exchangePanel;
    public GameObject playerTradePanel;

    public void ShowTradingPanel(Player localPlayer, Player[] playersToTradeWith, ExchangeHandler exchangeHandler, TradeHandler tradeHandler, TradeCancellation onTradeCancel) {
        this.localPlayer = localPlayer;
        this.playersToTradeWith = playersToTradeWith;
        onExchange = exchangeHandler;
        onTradeRequestSent = tradeHandler;
        this.onTradeCancel = onTradeCancel;

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
        // Should not able able to open player trade panel if there is no players to trade with
        if(playersToTradeWith.Length == 0) {
            return;
        }

        exchangePanel.SetActive(false);
        playerTradePanel.SetActive(true);
        playerTradePanel.GetComponent<PlayerTradeViewController>().Initialize(localPlayer, playersToTradeWith, (playerToTradeWith, resourceA, resourceB) => {
            if(onTradeRequestSent != null) {
                canCancelWithESC = false;
                onTradeRequestSent(playerToTradeWith, resourceA, resourceB);
            }
        }, (player) => {
            if(onTradeCancel != null) {
                onTradeCancel(player);
                canCancelWithESC = true;
            }
        });
    }

    public void Disable() {
        var tradeRequestViewController = GetComponentInChildren<TradeRequestViewController>();
        if(tradeRequestViewController != null) {
            tradeRequestViewController.Show(false);
        }

        playerTradePanel.SetActive(false);
        exchangePanel.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Update() {
        if(!gameObject.activeSelf || !canCancelWithESC) {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            Disable();
        }
    }
}
