using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MapController mapController;
    public List<Location> locations;
    public List<Path> paths;

    public enum State {
        
    }
    
    [Header("Resources")]
    public int wood = 0;
    public int stone = 0;
    public int cobber = 0;
    public int wheat = 0;
    public int wool = 0;

    public void Start() {
        locations = new List<Location>();
        paths = new List<Path>();
        mapController = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapController>();
    }
}
