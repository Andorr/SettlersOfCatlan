using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public class Map 
    {
        public Player[] players;
        public Dictionary<int, Location> locations;
        public Dictionary<int, Tile> tiles;
        public Dictionary<int, Path> paths;

        public Map() {
            locations = new Dictionary<int, Location>();
            tiles = new Dictionary<int, Tile>();
            paths = new Dictionary<int, Path>();
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
    }


}
