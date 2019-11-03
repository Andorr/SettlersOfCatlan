using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public interface IMapClient
{
    Location[] GetAdjecentLocations(Location location);
}
