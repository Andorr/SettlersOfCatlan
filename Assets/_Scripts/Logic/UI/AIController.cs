using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour  {

    public MapController mapController;

    void Start () {
        mapController = GetComponent<MapController>();
        mapController.GenerateMap();
        Debug.Log("Hello, AI!");   
        Debug.Log(mapController.GetMap().locations);
        // Calculate a value for each of the locations. 

            // Need to figure out the potential earnings of the tile.

            // Each resource should have a value. Will probably be dynamic based on what the player currently is in need of.

            // Having a lot of a resource makes it less valueable.

        // Calculate a value of each of the paths. 

            // Paths that make the current longest path longer of gets closer to the next ... is good. 

        // Calculate a value for moving the worker to each of the locations.

            // Moving a worker closer to something that should be buildt is good.
            
            // Will maybe need an order queue.

        // Calcuate the value of buying a card.

            // Will only be a thing if there are enought resources.

            // Saving up for a card should also be a possiblity. 

        // Choose the action with the highest value.

            // It's that simple.
    }

}