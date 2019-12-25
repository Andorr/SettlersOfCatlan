using State;
using UnityEngine;

public static class ResourceUtil
{
    public static bool CanAffordHouse(ResourceStorage storage)
    {
        return storage.wood >= 1 && storage.wheat >= 1 && storage.wool >= 1 && storage.clay >= 1;
    }

    public static bool CanAffordCity(ResourceStorage storage)
    {
        return storage.stone >= 3 && storage.wheat >= 2;
    }

    public static bool CanAffordPath(ResourceStorage storage)
    {
        return storage.clay >= 1 && storage.wood >= 1;
    }
    public static bool CanAffordCard(ResourceStorage storage){
        return storage.wool >= 1 && storage.stone >= 1 && storage.wheat >= 1;
    }

    public static void PurchaseHouse(ResourceStorage storage)
    {
        storage.wood = Mathf.Max(0, storage.wood - 1);
        storage.wheat = Mathf.Max(0, storage.wheat - 1);
        storage.wool = Mathf.Max(0, storage.wool - 1);
        storage.clay = Mathf.Max(0, storage.clay - 1);
    }

    public static void PurchaseCity(ResourceStorage storage)
    {
        storage.stone = Mathf.Max(0, storage.stone - 3);
        storage.wheat = Mathf.Max(0, storage.wheat - 2);
    }

    public static void PurchasePath(ResourceStorage storage)
    {
        storage.clay = Mathf.Max(0, storage.clay - 1);
        storage.wood = Mathf.Max(0, storage.wood - 1);
    }

    public static void PurcahseCard(ResourceStorage storage){
        storage.wool = Mathf.Max(0,storage.wool-1);
        storage.stone = Mathf.Max(0, storage.stone - 1);
        storage.wheat = Mathf.Max(0, storage.wheat - 1);
    }

    public static string TypeToString(ResourceType type) {
        if(type == ResourceType.Wood) {
            return "wood";
        } else if(type == ResourceType.Stone) {
            return "stone";
        } else if(type == ResourceType.Clay) {
            return "clay";
        } else if(type == ResourceType.Wheat) {
            return "wheat";
        } else if(type == ResourceType.Wool) {
            return "wool";
        }
        return null;
    }
}
