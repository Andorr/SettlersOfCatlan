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
        }
    }

    public override string ToString() {
        return $"{wood} wood, {stone} stone, {clay} clay, {wheat} wheat and {wool} wool";
    }
}

public enum ResourceType {
    Wood = 0,
    Stone = 1,
    Clay = 2,
    Wheat = 3,
    Wool = 4,
}