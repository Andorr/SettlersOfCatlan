using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    [System.Serializable]
    public enum LocationType {
        Available = 0,
        House = 1,
        City = 2,
    }

    [System.Serializable]
    public class Location 
    {
        public int id;
        public LocationType type;
        public Vector3 position;
        public Player occupiedBy;
    }
}
