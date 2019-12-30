using State;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerTradeViewController : MonoBehaviour
{
    private ResourceStorage fromStorage;
    private ResourceStorage toStorage;
    
    private ResourceStorage fromSelected;
    private ResourceStorage toSelected;

    private Player[] players;
    private Player selectedPlayer;

    public delegate void TradeHandler(Player playerToTradeWith, ResourceStorage a, ResourceStorage b);
    public delegate void TradeCancellation(Player player);
    private TradeHandler onTradeRequested;
    private TradeCancellation onCancel;
    private bool waitingForAnswer;

    [Header("Players")]
    public GameObject playerPrefab;
    public GameObject playerHolder;
    public Text currentPlayerText;

    [Header("Panels")]
    public GameObject tradeSelectPanel;
    public GameObject tradeViewPanel;
    public GameObject resourceItemPrefab;

    [Header("Resource Buttons")]
    public Button requestTradeBtn;
    public GameObject[] from;
    public GameObject[] to;

    [Header("Sprites")]
    public Sprite woodSprite;
    public Sprite stoneSprite;
    public Sprite claySprite;
    public Sprite wheatSprite;
    public Sprite woolSprite;

    public void Start() {
        InitializeButtons();
    }

    public void Initialize(Player localPlayer, Player[] playersToTradeWith, TradeHandler handler, TradeCancellation onCancel) {
        fromStorage = localPlayer.resources;
        fromSelected = new ResourceStorage();
        toSelected = new ResourceStorage();
        players = playersToTradeWith;
        waitingForAnswer = false;
        InitializePlayers();
        ChangePlayer(players[0]);
        ShowTradeView(false);
        onTradeRequested = handler;
        this.onCancel = onCancel;
    }

    public void OnTradeRequestClick() {
        if(waitingForAnswer) {
            ShowTradeView(false);
            if(onCancel != null) {
                onCancel(selectedPlayer);
                waitingForAnswer = false;
            }
            return;
        }


        if(onTradeRequested != null && !fromSelected.IsEmpty() && !toSelected.IsEmpty()) {
            waitingForAnswer = true;
            onTradeRequested(selectedPlayer, fromSelected, toSelected);
            ShowTradeView(true);
        }
    }

    private void UpdateFromView() {
        var resources = new int[]{fromStorage.wood, fromStorage.stone, fromStorage.clay, fromStorage.wheat, fromStorage.wool};
        var selected = new int[]{fromSelected.wood, fromSelected.stone, fromSelected.clay, fromSelected.wheat, fromSelected.wool};

        for(int i = 0; i < from.Length; i++) {
            from[i].GetComponent<LeftRightClickable>().Interactable(resources[i] > 0);
            from[i].transform.GetChild(0).GetComponent<Text>().text = resources[i].ToString();
            from[i].transform.GetChild(1).GetComponent<Text>().text = selected[i].ToString();
        }

        UpdateButton();
    }

    private void UpdateToView() {
        var resources = new int[]{toStorage.wood, toStorage.stone, toStorage.clay, toStorage.wheat, toStorage.wool};
        var selected = new int[]{toSelected.wood, toSelected.stone, toSelected.clay, toSelected.wheat, toSelected.wool};

        for(int i = 0; i < to.Length; i++) {
            to[i].GetComponent<LeftRightClickable>().Interactable(resources[i] > 0);
            to[i].transform.GetChild(0).GetComponent<Text>().text = resources[i].ToString();
            to[i].transform.GetChild(1).GetComponent<Text>().text = selected[i].ToString();
        }

        UpdateButton();
    }

    private void UpdateButton() {
        if(!fromSelected.IsEmpty() && !toSelected.IsEmpty()) {
            requestTradeBtn.interactable = true;
        } else {
            requestTradeBtn.interactable = false;
        }
    }

    private void InitializeButtons() {
        for(int i = 0; i < from.Length; i++) {
            int resourceType = i;
            var clickable = from[i].GetComponent<LeftRightClickable>();
            clickable.onLeft.AddListener(() => {
                AddResource(fromStorage, fromSelected, resourceType, 1);
                UpdateFromView();
            });
            clickable.onRight.AddListener(() => {
                AddResource(fromStorage, fromSelected, resourceType, -1);
                UpdateFromView();
            });
        }

        for(int i = 0; i < to.Length; i++) {
            int resourceType = i;
            var clickable = to[i].GetComponent<LeftRightClickable>();
            clickable.onLeft.AddListener(() => {
                AddResource(toStorage, toSelected, resourceType, 1);
                UpdateToView();
            });
            clickable.onRight.AddListener(() => {
                AddResource(toStorage, toSelected, resourceType, -1);
                UpdateToView();
            });
        }
    }

    private void AddResource(ResourceStorage storage, ResourceStorage selected, int value, int quantity) {
        ResourceType type = ResourceUtil.IntToType(value);

        // Check if the user has enough resources to increase
        if(quantity > 0 && !storage.HasResource(type, selected.GetResource(type) + quantity)) {
            return;
        }

        selected.AddResource(type, quantity);
    }

    private void InitializePlayers() {
        // Delete all the players objects from the player holder
        foreach(Transform child in playerHolder.transform) {
            Destroy(child.gameObject);
        }

        // Initialize player button for all players to trade with
        foreach(Player p in players) {
            var s = GameObject.Instantiate(playerPrefab);
            s.GetComponentInChildren<Text>().text = p.name.Length > 2 ? p.name.Substring(0, 2) : p.name;
            s.GetComponent<Image>().color = p.GetColor();
            s.GetComponent<Button>().onClick.AddListener(() => {
                ChangePlayer(p);
            });
            s.transform.SetParent(playerHolder.transform);
        }
    }

    private void ChangePlayer(Player p) {
        if(waitingForAnswer) {
            return;
        }

        selectedPlayer = p;
        toStorage = p.resources;
        toSelected = new ResourceStorage();
        currentPlayerText.text = $"Trade with {p.name}";
        UpdateFromView();
        UpdateToView();
    }

    private void ShowTradeView(bool enable) {
        tradeSelectPanel.SetActive(!enable);
        tradeViewPanel.SetActive(enable);

        if(enable) {
            UpdateResourceView(tradeViewPanel.transform.GetChild(0).gameObject, fromSelected);
            UpdateResourceView(tradeViewPanel.transform.GetChild(2).gameObject, toSelected);
            currentPlayerText.text = "Waiting for answer";
            requestTradeBtn.GetComponentInChildren<Text>().text = "Cancel trade";
        } else {
            requestTradeBtn.GetComponentInChildren<Text>().text = "Request trade";
            currentPlayerText.text = $"Trade with {selectedPlayer.name}";
            UpdateButton();
        }
    }

    private void UpdateResourceView(GameObject holder, ResourceStorage storage) {
        // Delete all children
        foreach(Transform child in holder.transform) {
            Destroy(child.gameObject);
        }

        var resources = new int[]{storage.wood, storage.stone, storage.clay, storage.wheat, storage.wool};
        for(int i = 0; i < resources.Length; i++) {
            if(resources[i] <= 0) {
                continue;
            }
            var resourceType = ResourceUtil.IntToType(i);
            var resourceItem = GameObject.Instantiate(resourceItemPrefab);
            resourceItem.transform.SetParent(holder.transform);
            resourceItem.GetComponentInChildren<Image>().sprite = TypeToSprite(resourceType);
            resourceItem.GetComponentInChildren<Text>().text = $"{resources[i]}x";
        }
    }
    
    private Sprite TypeToSprite(ResourceType type) {
        switch(type) {
            case ResourceType.Wood: {
                return woodSprite;
            }
            case ResourceType.Stone: {
                return stoneSprite;
            }
            case ResourceType.Clay: {
                return claySprite;
            }
            case ResourceType.Wheat: {
                return wheatSprite;
            }
            case ResourceType.Wool: {
                return woolSprite;
            }
        }
        return null;
    }
}
