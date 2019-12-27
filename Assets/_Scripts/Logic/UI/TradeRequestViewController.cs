using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using UnityEngine.UI;

public class TradeRequestViewController : MonoBehaviour
{
    public delegate void RequestHandler(bool accepted, Player playerToTradeWith, ResourceStorage from, ResourceStorage to);
    private RequestHandler onActionDone;
    private ResourceStorage fromStorage;
    private ResourceStorage toStorage;
    private Player playerToTradeWith;

    [Header("GameObjects")]
    public GameObject resourceItemPrefab;
    public GameObject resourcesFromHolder;
    public GameObject resourcesToHolder;

    [Header("UI")]
    public Text playerText;

    [Header("Sprites")]
    public Sprite woodSprite;
    public Sprite stoneSprite;
    public Sprite claySprite;
    public Sprite wheatSprite;
    public Sprite woolSprite;

    public void Initialize(Player playerToTradeWith, ResourceStorage from, ResourceStorage to, RequestHandler handler)
    {
        onActionDone = handler;
        fromStorage = from;
        toStorage = to;
        this.playerToTradeWith = playerToTradeWith;
        Show(true);
        UpdateView();
    }

    public void Show(bool show) {
        gameObject.transform.parent.gameObject.SetActive(show);
        gameObject.SetActive(show);
    }

    public void onCancelBtnClicked() {
        if(onActionDone != null) {
            onActionDone(false, playerToTradeWith, fromStorage, toStorage);
        }
    }

    public void onAcceptBtnClicked() {
        if(onActionDone != null) {
            onActionDone(true, playerToTradeWith, fromStorage, toStorage);
        }
    }

    private void UpdateView() {
        UpdateResourceView(resourcesFromHolder, fromStorage);
        UpdateResourceView(resourcesToHolder, toStorage);

        playerText.text = $"<color={ColorUtility.ToHtmlStringRGB(playerToTradeWith.GetColor())}>{playerToTradeWith.name}</color> wants to trade with you.";
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
