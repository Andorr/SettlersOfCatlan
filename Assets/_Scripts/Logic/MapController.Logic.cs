using System.Collections;
using System.Collections.Generic;
using System.Linq;
using State;
using UnityEngine;

public partial class MapController
{
    public Location[] GetAdjecentLocations(Location location, bool mustBeAvailable = false) {
        // Get the paths connected to the location
        var connectedPaths = map.paths.Values.Where(path => path.between.Item1.id == location.id || path.between.Item2.id == location.id);

        // The the locations from the paths
        var locations = connectedPaths.Select((path) => path.between.Item1.id == location.id ? path.between.Item2 : path.between.Item1);

        // Filter away unavailable locations if not there
        if(mustBeAvailable) {
            locations = locations.Where((l) => l.type == LocationType.Available);
        }

        return locations.ToArray();
    }

    public void EnableLocationBoxColliders(bool enable)
    {
        foreach(var location in locations.Values) {
            location.GetComponent<BoxCollider>().enabled = enable;
        }
    }
}
