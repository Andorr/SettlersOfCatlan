using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using System.Linq;

public class MapClient : IMapClient
{
    private readonly Map map;

    public MapClient(Map map)
    {
        this.map = map;
    }

    public Location[] GetAdjecentLocations(Location location)
    {
        Dictionary<int, Location> adjecentLocations = new Dictionary<int, Location>();
        
        return null;
    }
}
