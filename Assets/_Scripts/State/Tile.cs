using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public enum TileType {
        Forest = 0, // Wood land
        Mountain = 1, // Stone land
        Field = 2, // Wheat land
        Pasture = 3, // Wool field
        Hill = 4, // Clay land
    }

    [System.Serializable]
    public class Tile
    {
        public int id;
        public TileType type;
        public Vector3 position;
        public Location[] locations;
    }
}