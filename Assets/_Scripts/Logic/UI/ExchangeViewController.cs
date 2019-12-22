using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeViewController : MonoBehaviour
{
    public delegate void ExchangeHandler(ResourceType from, ResourceType to);
    private ResourceType fromResource = ResourceType.Wood;
    private ResourceType toResource = ResourceType.Stone;
    private ResourceStorage storage;
    private ExchangeHandler onExchange;
    
    [Header("UI Components")]
    public Button exchangeButton;
    public Image fromResourceImage;
    public Image toResourceImage;

    [Header("Sprites")]
    public Sprite woodSprite;
    public Sprite stoneSprite;
    public Sprite claySprite;
    public Sprite wheatSprite;
    public Sprite woolSprite;

    public void Start() {
        SetFromResource(ResourceType.Wood);
        SetToResource(ResourceType.Stone);
    }

    public void Initialize(ResourceStorage storage, ExchangeHandler handler) {
        this.storage = storage;
        this.onExchange = handler;
    }

    private void SetFromResource(ResourceType type) {
        fromResource = type;
        fromResourceImage.sprite = TypeToSprite(type);
        ValidateExchange(type);
    }

    private void SetToResource(ResourceType type) {
        toResource = type;
        toResourceImage.sprite = TypeToSprite(type);
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

    public void OnFromResourceClicked(int resourceValue) {
        var type = (ResourceType)resourceValue;
        SetFromResource(type);
    }

    public void OnToResourceClicked(int resourceValue) {
        var type = (ResourceType)resourceValue;
        SetToResource(type);
    }

    private void ValidateExchange(ResourceType type) {
        if(storage.HasResource(type, 3)) {
            exchangeButton.interactable = true;
            exchangeButton.GetComponentInChildren<Text>().text = "Exchange!";
        } else {
            exchangeButton.interactable = false;
            exchangeButton.GetComponentInChildren<Text>().text = "Not enough resources";
        }
    }

    public void Exchange() {      
        if(onExchange != null) {
            onExchange(fromResource, toResource);
        }
    }
}
