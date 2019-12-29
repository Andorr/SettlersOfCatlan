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
    public delegate void OnPlayerSelect(Player player);
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
    public GameObject tabs;
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
        EnableClosability(true);
    }

    public void EnablePlayerSelect(Player[] players, OnPlayerSelect callback) {
        gameObject.SetActive(true);
        playerSelectPanel.SetActive(true);
        EnableClosability(false);

        // Delete previous gameObjects
        foreach(Transform child in playerSelectPanel.transform.GetChild(1).transform) {
            Destroy(child.gameObject);
        }

        foreach(Player p in players) {
            GameObject obj = GameObject.Instantiate(playerCardPrefab, Vector3.zero, Quaternion.identity);
            obj.GetComponentInChildren<Text>().text = p.name;
            obj.transform.SetParent(playerSelectPanel.transform.GetChild(1));
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => {
                callback(p);
            });
            obj.GetComponent<EventTrigger>().triggers.Add(entry);
        }
    }

    public void DisablePlayerSelect() {
        gameObject.SetActive(false);
        playerSelectPanel.SetActive(false);
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
                onTradeRequestSent(playerToTradeWith, resourceA, resourceB);
                EnableClosability(false);
            }
        }, (player) => {
            if(onTradeCancel != null) {
                onTradeCancel(player);
                EnableClosability(true);
            }
        });
    }

    private void EnableClosability(bool enable) {
        canCancelWithESC = enable;
        foreach(Button b in tabs.GetComponentsInChildren<Button>()) {
            b.interactable = enable;
        }
    }

    public void Disable() {
        var tradeRequestViewController = GetComponentInChildren<TradeRequestViewController>();
        if(tradeRequestViewController != null) {
            tradeRequestViewController.Show(false);
        }

        playerSelectPanel.SetActive(false);
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
