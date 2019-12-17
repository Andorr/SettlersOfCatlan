using System.Collections;
using System.Collections.Generic;
using System.Linq;
using State;
using UnityEngine;

public partial class MapController 
{
    public (int wood, int stone, int clay, int wheat, int wool) CalculateGainableResources(Player player) {
        
        // Get the count where the player has one or more locations for each tile => (tileId, player's locations)
        var tileLocationCounts = map.tiles.Values
            .Select(t => (t.id, t.locations.Where(l => l.occupiedBy != null && l.occupiedBy.id.Equals(player.id))));
        

        int wood = 0;
        int stone = 0;
        int clay = 0;
        int wheat = 0;
        int wool = 0;
        // Foreach tile, get the count the number of locations and add to corresponding type
        foreach(var (tileId, playerLocations) in tileLocationCounts) {
            int resourcesToAdd = 0;
            foreach(var l in playerLocations) {
                // Two resource for a city and one for a house
                resourcesToAdd += l.type == LocationType.City ? 2 : l.type == LocationType.House ? 1 : 0;
            }


            var type = map.tiles[tileId].type;
            switch(type) {
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
