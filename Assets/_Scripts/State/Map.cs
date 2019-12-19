using System.Collections;
using System.Collections.Generic;
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
    }


}
