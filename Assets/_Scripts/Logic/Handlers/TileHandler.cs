using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class TileHandler : MonoBehaviour, IActionHandler
{
    public bool TryOverride => false;

    private TileController tileController;


    public void Start() {
        tileController = GetComponent<TileController>();
    }

    public void OnHover(GameController controller)
    {
        
    }

    public void OnSelected(GameController controller)
    {
        Debug.Log("Tile pressed");
        // // Move/create thief to selected tile
        // // #1 Get tile with thief from GameController
        // TileController thiefTile = controller.GetThiefTile();
        // bool thiefExists = thiefTile != null;
        // // #2   if it exists use shrink animation
        // if (thiefExists) {
        //     // Get the tile the thief exists on
        //     // Call remove thief from tile
        //     thiefTile.RemoveThief(controller);
        // }
        // // #3 Create thief on new tile and use grow animation
        // tileController.AddThief(controller);

        int newTileId = tileController.tile.id;
        controller.GetLocalPlayer().MoveThief(newTileId);

        foreach (var tile in controller.mapController.GetAllTileControllers()) {
            tile.SetSelectable(false);
        }
    }

    public bool OnTryOverride(GameController controller, GameObject other)
    {
        return true;
    }

    public void OnUnselected(GameController controller)
    {
        
    }
}