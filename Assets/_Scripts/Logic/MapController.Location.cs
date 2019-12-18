using System.Collections.Generic;
using System.Linq;
using State;
using UnityEngine;

public partial class MapController
{
    public Location GetLocationById(int id) {
        return map.locations[id];
    }

    public bool GetLocationController(Location location, out LocationController lc)
    {
        var hasController = locations.TryGetValue(location.id, out GameObject obj);
        if(!hasController) {
            lc = null;
            return false;
        }

        lc = obj.GetComponent<LocationController>();
        return lc != null;
    }

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

    public LocationController[] GetReachableLocations(Location startLocation, Player player) {
       
        // Check if other locations are reachable from the adjecent locations through paths
        var stack = new Stack<Location>();
        var result = new Dictionary<int, LocationController>();

        // Deapth first search to find all the locations that are available between the player's paths
        Location current = startLocation;
        while(current != null) {
            if(result.ContainsKey(current.id)) {
                current = stack.Count > 0 ? stack.Pop() : null;
                continue;
            }

            if(locations.ContainsKey(current.id)) {
                result.Add(current.id, locations[current.id].GetComponent<LocationController>());
            }

            // Get all reachable locations from a location
            var paths = map.paths.Values.Where(p => p.between.Item1.id == current.id || p.between.Item2.id == current.id);
            foreach(Path p in paths) {
                // Check if path has a road on it and it belongs to given player
                if(p.occupiedBy == null || p.occupiedBy.id != player.id) {
                    continue;
                } 

                // Get the location and add it to the stack
                var reachableLocation = p.between.Item1.id == current.id ? p.between.Item2 : p.between.Item1;
                if(reachableLocation.id == startLocation.id || (reachableLocation.occupiedBy != null && !reachableLocation.occupiedBy.id.Equals(player.id))) {
                    continue;
                }
                stack.Push(reachableLocation);
            }

            current = stack.Count > 0 ? stack.Pop() : null;
        }

        // Add adjecent locations if there is no opponent locations around
        var adjecentLocations = map.paths.Values
            .Where(p => (p.between.Item1.id == startLocation.id || p.between.Item2.id == startLocation.id)
                     && (p.occupiedBy == null || p.occupiedBy.id.Equals(player.id)))
            .Select(p => p.between.Item1.id == startLocation.id ? p.between.Item2 : p.between.Item1)
            .Where(l => l.occupiedBy == null || l.occupiedBy.id.Equals(player.id));

        foreach(Location l in adjecentLocations) {
            if(!result.ContainsKey(l.id)) {
                LocationController lc = locations[l.id].GetComponent<LocationController>();
                result.Add(l.id, lc);
            }
        }

        // Remove the start location
        result.Remove(startLocation.id);

        return result.Values.ToArray();
    }

    public void EnableLocationBoxColliders(bool enable)
    {
        foreach(var location in locations.Values) {
            location.GetComponent<BoxCollider>().enabled = enable;
        }
    }
}
