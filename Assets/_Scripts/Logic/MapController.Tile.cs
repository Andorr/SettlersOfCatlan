using System.Collections.Generic;
using System.Linq;
using State;
using UnityEngine;

public partial class MapController{
    public TileController[] GetAllTileControllers() {
        return map.tiles.Values.Select(t => tiles[t.id].GetComponent<TileController>()).ToArray();
    }

    public TileController GetTileControllerById(int id) {
        return tiles[id].GetComponent<TileController>();
    }

    public void EnableTileBoxColliders(bool enable) {
        foreach(var tileController in GetAllTileControllers()) {
            tileController.SetSelectable(enable);
        }
    }
}