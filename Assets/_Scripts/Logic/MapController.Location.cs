using System.Collections;
using System.Collections.Generic;
using System.Linq;
using State;
using UnityEngine;

public partial class MapController
{
    public LocationController[] GetAdjecentLocations(Location location, bool mustBeAvailable = false) {
        // Get the paths connected to the location
        var connectedPaths = map.paths.Values.Where(path => path.between.Item1.id == location.id || path.between.Item2.id == location.id);

        // The locations from the paths
        var adjecentLocations = connectedPaths.Select((path) => path.between.Item1.id == location.id ? path.between.Item2 : path.between.Item1);

        // Get the controllers
        var controllers = adjecentLocations.Select(l => locations[l.id].GetComponent<LocationController>());

        // Filter away unavailable locations if not there
        if(mustBeAvailable) {
            controllers = controllers.Where((l) => l.location.type == LocationType.Available);
        }

        return controllers.ToArray();
    }

    public void EnableLocationBoxColliders(bool enable)
    {
        foreach(var location in locations.Values) {
            location.GetComponent<BoxCollider>().enabled = enable;
        }
    }
}
