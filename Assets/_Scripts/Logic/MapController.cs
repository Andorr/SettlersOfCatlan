using System;
using State;
using UnityEngine;
using static MapUtil;

public partial class MapController : MonoBehaviour
{
    private Map map;

    [Header("GameObjects")]
    public GameObject tilePrefab;
    public GameObject locationPrefab;
    private Transform mapHolder;

    [Header("Map Configurations")]
    public float radius = 3f;
    public int size = 9;
    public MapShape shape = MapShape.HexagonalLattice;
    public TileGeneration generation = TileGeneration.Random;

    public void GenerateMap()
    {
        // Delete existing map from the scene
        if(mapHolder != null) {
            DestroyImmediate(mapHolder.gameObject);
        }

        GameObject mapParent = new GameObject();
        mapParent.transform.position = Vector3.zero;
        mapParent.name = "Map";
        mapHolder = mapParent.transform;

        map = MapUtil.GenerateMap(size, radius, shape, generation);

        // Visualize tiles
        GameObject tileParent = new GameObject();
        tileParent.transform.SetParent(mapParent.transform);
        tileParent.transform.name = "Tiles";
        foreach(Tile t in map.tiles.Values) {
            GameObject g = GameObject.Instantiate(tilePrefab, t.position, Quaternion.Euler(0, 0, 0));
            TileController tileController = g.GetComponent<TileController>();
            tileController.Initialize(t, radius);
            g.transform.SetParent(tileParent.transform);
        }

        // Visualize locations
        GameObject locationParent = new GameObject();
        locationParent.transform.SetParent(mapParent.transform);
        locationParent.name = "Locations";
        foreach(Location l in map.locations.Values) {
            GameObject location = GameObject.Instantiate(locationPrefab, l.position, Quaternion.identity);
            location.GetComponent<LocationController>().Initialize(l, radius);
            location.transform.SetParent(locationParent.transform);
        }

        // Visualize paths
        GameObject pathParent = new GameObject();
        pathParent.transform.SetParent(mapParent.transform);
        pathParent.name = "Paths";
        foreach(Path p in map.paths.Values) {
            // Calculate the position between the locations
            Vector3 dir = p.between.Item1.position - p.between.Item2.position;
            Vector3 pathPos = p.between.Item2.position + dir*0.5f;

            GameObject loc = GameObject.CreatePrimitive(PrimitiveType.Cube);
            loc.transform.position = new Vector3(pathPos.x, 0.5f, pathPos.z);
            loc.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            loc.transform.SetParent(pathParent.transform);
        }
    }
}