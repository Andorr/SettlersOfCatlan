using UnityEngine;
using System.Collections;
using State;
using System.Collections.Generic;
using System;

public class AIController : MonoBehaviour  {

    public MapController mapController;
    public Map map;
    public string me;
    private bool hasComputed;


    void Update () {
        if (hasComputed) {
            return;
        }
        mapController = GetComponent<GameController>().mapController;
        Debug.Log("Hello, AI!");   
        map = mapController.GetMap();
        Debug.Log(map);
           
        var locationValues = new Dictionary<Location, double>();
        var pathValues = new Dictionary<Path, double>();
        
        // Calculate a value for each of the locations. 
        foreach (var l in map.locations) {
            var location = l.Value;

            var resources = new Dictionary<TileType, double>();

            // Need to figure out the potential earnings of the locations.
            foreach (var t in map.tiles) {
                var tile = t.Value;
                resources[tile.type] += Probability(tile.value);
            }

            // Each resource should have a value. Will probably be dynamic based on what the player currently is in need of.
            // Having a lot of a resource makes it less valueable.
            var value = 0.0;
            foreach (var r in resources) {
                value += r.Value * Modifier(r.Key);
            }

            locationValues[location] = value;
        }

        // Calculate a value of each of the paths. 
        foreach (var p in map.paths) {
            var path = p.Value;

            // If we can not build there we don't need to consider it.
            if (path.occupiedBy != null || (path.between.Item1.occupiedBy != me && path.between.Item2.occupiedBy != me))
                continue;

            // Paths that make the current longest path longer of gets closer to the next ... is good. 
            var queue = new Queue<Tuple<Location, int>>();
            queue.Enqueue(new Tuple<Location, int>(path.between.Item1, 0));
            queue.Enqueue(new Tuple<Location, int>(path.between.Item2, 0));

            var totalValue = 0.0;

            while (queue.Count > 0) {
                var item = queue.Dequeue();
                var location = item.Item1;
                var distance = item.Item2;
                if (location.occupiedBy != null) {
                    continue;
                }
                totalValue += locationValues[location] / distance;
                foreach (var target in ReachableFrom(location)) {
                    queue.Enqueue(new Tuple<Location, int>(target, distance + 1));
                }
            }

            pathValues[path] = totalValue;
        }

        // Calculate a value for moving the worker to each of the locations.
        foreach (var w in map.workers) {
            var  worker = w.Value;

            // Moving a worker closer to something that should be buildt is good.
            
            // Will maybe need an order queue.

        }

        // Calcuate the value of buying a card.

            // Will only be a thing if there are enought resources.

            // Saving up for a card should also be a possiblity. 

        // Choose the action with the highest value.

            // It's that simple.
        
        hasComputed = true;
    }

    // Returns the probability of getting a given number each turn.
    public double Probability(int number) {
        return 1.0;
    }

    // Returns a modifier given to the given type based on the current situation. 
    public double Modifier(TileType type) {
        // Should take into account the value of the type at this stage of the game, the current goals of the AI
        // and the amount of that resource the AI needs.
        return 1.0;
    }

    public List<Location> ReachableFrom(Location location) {
        return new List<Location>();
    }

}