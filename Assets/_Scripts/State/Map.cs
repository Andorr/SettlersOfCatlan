using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace State
{
    public class Map 
    {
        public Dictionary<string, Player> players;
        public Dictionary<int, Location> locations;
        public Dictionary<int, Tile> tiles;
        public Dictionary<int, Path> paths;
        public Dictionary<string, Worker> workers;

        public Map() {
            locations = new Dictionary<int, Location>();
            tiles = new Dictionary<int, Tile>();
            paths = new Dictionary<int, Path>();
            workers = new Dictionary<string, Worker>();
            players = new Dictionary<string, Player>();
        }

        public void AddLocations(IEnumerable<Location> _locations) {
            foreach(Location l in _locations) {
                locations.Add(l.id, l);
            }
        }

        public void AddTiles(IEnumerable<Tile> _tiles) {
            foreach(Tile l in _tiles) {
                tiles.Add(l.id, l);
            }
        }

        public void AddPaths(IEnumerable<Path> _paths) {
            foreach(Path l in _paths) {
                paths.Add(l.id, l);
            }
        }

        public void AddWorker(Worker worker) {
            workers.Add(worker.id, worker);
        }

        public void AddPlayer(Player player) {
            players.Add(player.id, player);
        }

        public int CalculateVictoryPoints(Player player) {
            int victoryPoints = 0;

            // Calculate victory points based on houses and cities
            var ownedStructures = locations.Values.Where(l => player.id.Equals(l.occupiedBy));
            var houseCount = ownedStructures.Where(l => l.type == LocationType.House).Count();
            var cityCount = ownedStructures.Where(l => l.type == LocationType.City).Count();
            victoryPoints += houseCount + cityCount*2; // One point for every house and two points for every city

            // TODO: Calculate victory points based on longest road

            // TODO: Calculate victory points based on Development cards
  

            player.victoryPoints = victoryPoints;
            return victoryPoints;
        }
    }


}
