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
        EnableExchangePanel();
        EnableClosability(true);
    }

    public void EnablePlayerSelect(Player[] players, OnPlayerSelect callback) {
        ActivePanel(playerSelectPanel.name);
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
                Disable();
            });
            obj.GetComponent<EventTrigger>().triggers.Add(entry);
            obj.GetComponent<Image>().color = p.GetColor();
        }
    }

    public void EnableExchangePanel() {
        ActivePanel(exchangePanel.name);
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

        ActivePanel(playerTradePanel.name);
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

    public void EnableClosability(bool enable) {
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

        ActivePanel(null);
    }

    public void Update() {
        if(!gameObject.activeSelf || !canCancelWithESC) {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) {
            Disable();
        }
    }

    private void ActivePanel(string panelName) {
        gameObject.SetActive(panelName != null);
        
        playerSelectPanel.SetActive(playerSelectPanel.name.Equals(panelName));

        playerTradePanel.SetActive(playerTradePanel.name.Equals(panelName));
        exchangePanel.SetActive(exchangePanel.name.Equals(panelName));
        tradePanel.SetActive(playerTradePanel.name.Equals(panelName) || exchangePanel.name.Equals(panelName));
    }
}
