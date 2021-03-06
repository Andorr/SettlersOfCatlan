﻿using System.Collections.Generic;
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
       
        var result = GetConnectedLocations(startLocation, player).ToDictionary(l => l.id);

        // Add adjecent locations if there is no opponent locations around
        var adjecentLocations = map.paths.Values
            .Where(p => (p.between.Item1.id == startLocation.id || p.between.Item2.id == startLocation.id)
                     && (p.occupiedBy == null || player.id.Equals(p.occupiedBy)))
            .Select(p => p.between.Item1.id == startLocation.id ? p.between.Item2 : p.between.Item1)
            .Where(l => l.occupiedBy == null || player.id.Equals(l.occupiedBy));

        foreach(Location l in adjecentLocations) {
            if(!result.ContainsKey(l.id)) {
                result.Add(l.id, l);
            }
        }

        // Remove the start location
        result.Remove(startLocation.id);

        return result.Values.Select(l => locations[l.id].GetComponent<LocationController>()).ToArray();
    }

    public List<Location> GetConnectedLocations(Location startLocation, Player player)
    {
        var stack = new Stack<Location>();
        var result = new Dictionary<int, Location>();

        // Deapth first search to find all the locations that are available between the player's paths
        Location current = startLocation;
        while(current != null) {
            if(result.ContainsKey(current.id)) {
                current = stack.Count > 0 ? stack.Pop() : null;
                continue;
            }

            if(locations.ContainsKey(current.id)) {
                result.Add(current.id, GetLocationById(current.id));
            }

            // Get all reachable locations from a location
            var paths = map.paths.Values.Where(p => p.between.Item1.id == current.id || p.between.Item2.id == current.id);
            foreach(Path p in paths) {
                // Check if path has a road on it and it belongs to given player
                if(p.occupiedBy == null || !player.id.Equals(p.occupiedBy)) {
                    continue;
                } 

                // Get the location and add it to the stack
                var reachableLocation = p.between.Item1.id == current.id ? p.between.Item2 : p.between.Item1;
                if(reachableLocation.id == startLocation.id || (reachableLocation.occupiedBy != null && !player.id.Equals(reachableLocation.occupiedBy))) {
                    continue;
                }
                stack.Push(reachableLocation);
            }

            current = stack.Count > 0 ? stack.Pop() : null;
        }

        return result.Values.ToList();
    }

    public void EnableLocationBoxColliders(bool enable)
    {
        foreach(var location in locations.Values) {
            location.GetComponent<BoxCollider>().enabled = enable;
        }
    }
}
