using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    [System.Serializable]
    public class Path
    {
        public int id;
        public Tuple<Location, Location> between;
        public Player occupiedBy;
    }

}
