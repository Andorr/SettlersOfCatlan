using State;
using UnityEngine;

public static class ResourceUtil
{
    public static bool CanAffordHouse(Player player)
    {
        return player.wood >= 1 && player.wheat >= 1 && player.wool >= 1 && player.clay >= 1;
    }

    public static bool CanAffordCity(Player player)
    {
        return player.stone >= 3 && player.wheat >= 2;
    }

    public static bool CanAffordPath(Player player)
    {
        return player.clay >= 1 && player.wood >= 1;
    }

    public static void PurchaseHouse(Player player)
    {
        player.wood = Mathf.Max(0, player.wood - 1);
        player.wheat = Mathf.Max(0, player.wheat - 1);
        player.wool = Mathf.Max(0, player.wool - 1);
        player.clay = Mathf.Max(0, player.clay - 1);
    }

    public static void PurchaseCity(Player player)
    {
        player.stone = Mathf.Max(0, player.stone - 3);
        player.wheat = Mathf.Max(0, player.wheat - 2);
    }

    public static void PurchasePath(Player player)
    {
        player.clay = Mathf.Max(0, player.clay - 1);
        player.wood = Mathf.Max(0, player.wood - 1);
    }
}
