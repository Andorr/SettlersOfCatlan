using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public enum TileType {
        Forest = 0,
        Mountain = 1,
        Arable = 2,
    }

    public class Tile
    {
        public int id;
        public TileType type;
        public Vector3 position;
        public Location[] locations;
    }
}