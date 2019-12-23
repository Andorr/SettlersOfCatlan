using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTradeViewController : MonoBehaviour
{
    private ResourceStorage fromStorage;
    private ResourceStorage toStorage;
    
    private ResourceStorage fromSelected;
    private ResourceStorage toSelected;

    private Player[] players;
    private Player selectedPlayer;

    public delegate void TradeHandler(ResourceStorage a, ResourceStorage b);
    public TradeHandler onTradeRequested;

    [Header("Players")]
    public GameObject playerPrefab;
    public GameObject playerHolder;
    public Text currentPlayerText;

    [Header("Resource Buttons")]
    public GameObject[] from;
    public GameObject[] to;

    public void Start() {
        InitializeButtons();
    }

    public void Initialize(Player localPlayer, Player[] playersToTradeWith) {
        fromStorage = localPlayer.resources;
        fromSelected = new ResourceStorage();
        toSelected = new ResourceStorage();
        InitializePlayers();
        ChangePlayer(playersToTradeWith[0]);
    }

    public void OnTradeRequestClick() {
        if(onTradeRequested != null) {
            onTradeRequested(fromSelected, toSelected);
        }
    }

    private void UpdateFromView() {
        var resources = new int[]{fromStorage.wood, fromStorage.stone, fromStorage.clay, fromStorage.wheat, fromStorage.wool};
        var selected = new int[]{fromSelected.wood, fromSelected.stone, fromSelected.clay, fromSelected.wheat, fromSelected.wool};

        for(int i = 0; i < from.Length; i++) {
            from[i].GetComponent<Button>().interactable = resources[i] > 0;
            from[i].transform.GetChild(0).GetComponent<Text>().text = resources[i].ToString();
            from[i].transform.GetChild(1).GetComponent<Text>().text = selected[i].ToString();
        }
    }

    private void UpdateToView() {
        var resources = new int[]{toStorage.wood, toStorage.stone, toStorage.clay, toStorage.wheat, toStorage.wool};
        var selected = new int[]{toSelected.wood, toSelected.stone, toSelected.clay, toSelected.wheat, toSelected.wool};

        for(int i = 0; i < to.Length; i++) {
            to[i].GetComponent<Button>().interactable = resources[i] > 0;
            to[i].transform.GetChild(0).GetComponent<Text>().text = resources[i].ToString();
            to[i].transform.GetChild(1).GetComponent<Text>().text = selected[i].ToString();
        }
    }

    private void InitializeButtons() {
        for(int i = 0; i < from.Length; i++) {
            var clickable = from[i].GetComponent<LeftRightClickable>();
            clickable.onLeft.AddListener(() => {
                AddResource(fromSelected, i, 1);
                UpdateFromView();
            });
            clickable.onRight.AddListener(() => {
                AddResource(fromSelected, i, -1);
                UpdateFromView();
            });
        }

        for(int i = 0; i < to.Length; i++) {
            var clickable = to[i].GetComponent<LeftRightClickable>();
            clickable.onLeft.AddListener(() => {
                AddResource(toSelected, i, 1);
                UpdateToView();
            });
            clickable.onRight.AddListener(() => {
                AddResource(toSelected, i, -1);
                UpdateToView();
            });
        }
    }

    private void AddResource(ResourceStorage storage, int value, int quantity) {
        ResourceType type = (ResourceType)value;

        if(value > 0 && !storage.HasResource(type, storage.GetResource(type) + value)) {
            return;
        }

        storage.AddResource(type, quantity);
    }

    private void InitializePlayers() {
        // Delete all the players objects from the player holder
        foreach(Transform child in playerHolder.transform) {
            Destroy(child.gameObject);
        }

        foreach(Player p in players) {
            var s = GameObject.Instantiate(playerPrefab);
            s.GetComponentInChildren<Text>().text = p.name.Length > 2 ? p.name.Substring(0, 2) : p.name;
            s.GetComponent<Image>().color = p.GetColor();
            s.GetComponent<Button>().onClick.AddListener(() => {
                ChangePlayer(p);
            });
        }
    }

    private void ChangePlayer(Player p) {
        selectedPlayer = p;
        toStorage = p.resources;
        currentPlayerText.text = $"Trade with <color={ColorUtility.ToHtmlStringRGB(p.GetColor())}>{p.name}</color>";
        UpdateFromView();
        UpdateToView();
    }


    
}
