using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : MonoBehaviour
{
    public int wood;
    public int stone;
    public int clay;
    public int wheat;
    public int wool;

    public bool IsEmpty() {
        return wood == 0 && stone == 0 && clay == 0 && wheat == 0 && wool == 0;
    }

    public bool HasResource(ResourceType type, int quantity) {
        if(ResourceType.Wood == type && wood >= quantity) {
            return true;
        } else if(ResourceType.Stone == type && stone >= quantity) {
            return true;
        } else if(ResourceType.Clay == type && clay >= quantity) {
            return true;
        } else if(ResourceType.Wheat == type && wheat >= quantity) {
            return true;
        } else if(ResourceType.Wool == type && wool >= quantity) {
            return true;
        }
        return false;
    }

    public int GetResource(ResourceType type) {
        if(ResourceType.Wood == type) {
            return wood;
        } else if(ResourceType.Stone == type) {
            return stone;
        } else if(ResourceType.Clay == type) {
            return clay;
        } else if(ResourceType.Wheat == type) {
            return wheat;
        } else if(ResourceType.Wool == type) {
            return wool;
        }
        return 0;
    }

    public void AddResource(ResourceType type, int value) {
        if(ResourceType.Wood == type) {
            wood = Mathf.Max(0, wood + value);
        } else if(ResourceType.Stone == type) {
            stone = Mathf.Max(0, stone + value);
        } else if(ResourceType.Clay == type) {
            clay = Mathf.Max(0, clay + value);
        } else if(ResourceType.Wheat == type) {
            wheat = Mathf.Max(0, wheat + value);
        } else if(ResourceType.Wool == type) {
            wool = Mathf.Max(0, wool + value);
        } else {
            Debug.Log("Invalid resource type: " + type);
        }
    }

    public void AddResources(ResourceStorage storage) {
        wood += storage.wood;
        stone += storage.stone;
        clay += storage.clay;
        wheat += storage.wheat;
        wool += storage.wool;
    }

    public override string ToString() {
        return $"{wood} wood, {stone} stone, {clay} clay, {wheat} wheat and {wool} wool";
    }

    public static ResourceStorage operator +(ResourceStorage a, ResourceStorage b) {
        a.wood += b.wood;
        a.stone += b.stone;
        a.clay += b.clay;
        a.wheat += b.wheat;
        a.wool += b.wool;
        return a;
    }

    public static byte[] Serialize(object customType) {
        var storage = (ResourceStorage)customType;
        return new byte[]{(byte)storage.wood, (byte)storage.stone, (byte)storage.clay, (byte)storage.wheat, (byte)storage.wool};
    }

    public static object Deserialize(byte[] data) {
        var storage = new ResourceStorage();
        storage.wood = (int)data[0];
        storage.stone = (int)data[1];
        storage.clay = (int)data[2];
        storage.wheat = (int)data[3];
        storage.wool = (int)data[4];
        return storage;
    }
}

public enum ResourceType {
    Wood = 0,
    Stone = 1,
    Clay = 2,
    Wheat = 3,
    Wool = 4,
}
