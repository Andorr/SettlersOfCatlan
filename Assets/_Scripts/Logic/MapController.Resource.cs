using System.Collections;
using System.Collections.Generic;
using System.Linq;
using State;
using UnityEngine;

public partial class MapController 
{
    public (int wood, int stone, int clay, int wheat, int wool) CalculateGainableResources(Player player, int thiefTileId) {
        
        // Get the count where the player has one or more locations for each tile => (tileId, player's locations)
        var tileLocationCounts = map.tiles.Values
            .Select(t => (t.id, t.locations.Where(l => l.occupiedBy != null && player.id.Equals(l.occupiedBy))));
        

        int wood = 0;
        int stone = 0;
        int clay = 0;
        int wheat = 0;
        int wool = 0;
        // Foreach tile, get the count the number of locations and add to corresponding type
        foreach(var (tileId, playerLocations) in tileLocationCounts) {
            var tile = map.tiles[tileId];
            int resourcesToAdd = 0;

            // If theif is on this tile, skip to next iteration of loop
            if (tileId == thiefTileId) continue;

            foreach(var l in playerLocations) {
                // Decide if the resource should given to the player from this location, based on randomness and the tile number
                var rollDice = Random.Range(1, 6) + Random.Range(1, 6); // Simulates the throw of two 5-sided dice. Number between 2-10

                // Two resource for a city and one for a house
                if(rollDice == tile.value) {
                    resourcesToAdd += l.type == LocationType.City ? 2 : l.type == LocationType.House ? 1 : 0;
                }
            }

            switch(tile.type) {
                case TileType.Field: {
                    wheat += resourcesToAdd;
                    break;
                }
                case TileType.Forest: {
                    wood += resourcesToAdd;
                    break;
                }
                case TileType.Hill: {
                    clay += resourcesToAdd;
                    break;
                }
                case TileType.Mountain: {
                    stone += resourcesToAdd;
                    break;
                }
                case TileType.Pasture: {
                    wool += resourcesToAdd;
                    break;
                }
            }
        }
        return (wood, stone, clay, wheat, wool);
    }
}
