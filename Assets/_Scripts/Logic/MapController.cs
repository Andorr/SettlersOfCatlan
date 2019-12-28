using System;
using System.Collections.Generic;
using State;
using UnityEngine;
using static MapUtil;

public partial class MapController : MonoBehaviour
{
    private Map map;
    private Dictionary<int, GameObject> locations;
    private Dictionary<int, GameObject> paths;

    private Dictionary<int, GameObject> tiles;

    [Header("GameObjects")]
    public GameObject tilePrefab;
    public GameObject locationPrefab;
    public GameObject workerPrefab;
    public GameObject pathPrefab;

    [Header("Map Configurations")]
    public float radius = 3f;
    public int size = 9;
    public MapShape shape = MapShape.HexagonalLattice;
    public TileGeneration generation = TileGeneration.Random;
    public int seed = 0;

    public void GenerateMap()
    {
        // Delete existing map from the scene
        GameObject existingMapHolder = GameObject.FindGameObjectWithTag("Map");
        if(existingMapHolder != null) {
            DestroyImmediate(existingMapHolder);
        }

        GameObject mapParent = new GameObject();
        mapParent.transform.position = Vector3.zero;
        mapParent.name = "Map";
        mapParent.tag = "Map";

        map = MapUtil.GenerateMap(size, radius, shape, generation, seed);
        locations = new Dictionary<int, GameObject>();
        paths = new Dictionary<int, GameObject>();
        tiles = new Dictionary<int, GameObject>();

        // Visualize tiles
        GameObject tileParent = new GameObject();
        tileParent.transform.SetParent(mapParent.transform);
        tileParent.transform.name = "Tiles";
        foreach(Tile t in map.tiles.Values) {
            GameObject g = GameObject.Instantiate(tilePrefab, t.position, Quaternion.Euler(0, 0, 0));
            TileController tileController = g.GetComponent<TileController>();
            tileController.Initialize(t, radius);
            g.transform.SetParent(tileParent.transform);
            tiles.Add(t.id, g);
        }

        // Visualize locations
        GameObject locationParent = new GameObject();
        locationParent.transform.SetParent(mapParent.transform);
        locationParent.name = "Locations";
        foreach(Location l in map.locations.Values) {
            GameObject location = GameObject.Instantiate(locationPrefab, l.position, Quaternion.identity);
            location.GetComponent<LocationController>().Initialize(l, radius);
            location.transform.SetParent(locationParent.transform);
            locations.Add(l.id, location);
        }

        // Visualize paths
        GameObject pathParent = new GameObject();
        pathParent.transform.SetParent(mapParent.transform);
        pathParent.name = "Paths";
        foreach(Path p in map.paths.Values) {
            // Calculate the position between the locations
            Vector3 dir = p.between.Item1.position - p.between.Item2.position;
            Vector3 pathPos = p.between.Item2.position + dir*0.5f;

            GameObject path = GameObject.Instantiate(pathPrefab, pathPos, Quaternion.identity);
            path.GetComponent<PathController>().Initialize(p, radius);
            path.transform.SetParent(pathParent.transform);
            paths.Add(p.id, path);
        }
    }

    public void AddWorkerToMap(Worker worker) {
        map.AddWorker(worker);
    }

    public void AddPlayerToMap(Player player) {
        map.AddPlayer(player);
    }

    public int CalculateVictoryPoints(Player player) {
        return map.CalculateVictoryPoints(player);
    }

    public Map GetMap() {
        return map;
    }

}
