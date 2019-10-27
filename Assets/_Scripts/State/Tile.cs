using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public enum TileType {
        Forest = 0,
        Mountain = 1,
        Field = 2, 
        Pasture = 3, // Grass field
        Hill = 4, 
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