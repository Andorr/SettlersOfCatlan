using System;
using System.Linq;
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
        int newTileId = tileController.tile.id;
        controller.GetLocalPlayer().MoveThief(newTileId);

        foreach (var tile in controller.mapController.GetAllTileControllers()) {
            tile.SetSelectable(false);
        }

        TradingViewController.OnPlayerSelect onPlayerSelect = (State.Player player) => {
            var from = player.resources;
            if (from.IsEmpty()) {
                // Stealing from player with no resources...
                controller.GetLocalPlayer().CannotStealResources(player.name);
                return;
            }
            controller.GetLocalPlayer().StealResourceFromPlayer(player);
        };

        var locationsToStealFrom = tileController.tile.locations.Where(location => location.type != State.LocationType.Available && location.occupiedBy != PhotonNetwork.LocalPlayer.UserId);
        if (locationsToStealFrom.Count() != 0) {
            if (locationsToStealFrom.Count() == 1) {
                // Only one player to steal from
                // Init card choosing ui? or just select a random card
                // eks. 'The thief stole 1 *insert resource* from *player name*'
                // #1 Find player with id
                // #2 call OnPlayerSelect callback with player
                var playerId = locationsToStealFrom.First().occupiedBy;
                controller.GetPlayers(out var otherPlayers);
                var onlyPlayerToStealFrom = otherPlayers.First();
                onPlayerSelect(onlyPlayerToStealFrom);
            } else {
                // More than one player to steal from, must select one
                // open ui to pick player and continue with same ui/resource text from player count 1
                controller.uiController.EnablePlayerPick(onPlayerSelect);
            }
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