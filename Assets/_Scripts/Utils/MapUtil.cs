using System.Collections.Generic;
using UnityEngine;
using State;
using System;
using System.Linq;

public static class MapUtil
{
    public enum MapShape {
        HexagonalLattice = 0,
        Doughnut = 1,
    }

    public enum TileGeneration {
        Random = 0,
    }

    public static Vector3[] HexagonalLattice(Vector2 origin, int size = 5, float radius = 1, float offset = 0)
    {
        if(size%2 == 0) {
            throw new ArgumentException("Size needs to be an odd number.");
        }

        // Calculating the distances
        float angle30 = Mathf.Deg2Rad*30;
        double xStep = Mathf.Cos(angle30)*(radius + offset);
        double yStep = Mathf.Sin(angle30)*(radius + offset);
  
        int half = size/2; // number of hexagons on the first row
        Vector3[] tiles = new Vector3[3*(half + 1)*(half) + 1];
        int i = 0;
        for(int row = 0; row < size; row++)
        {
            int numberOfCols = size - Mathf.Abs(row - half);
            float xOffset = (float)(numberOfCols % 2 == 0 ? xStep : 0);
            for(int col = 0; col < numberOfCols; col++)
            {
                float xPos = (float)(origin.x + xStep*2*(-numberOfCols/2 + col) + xOffset);
                float yPos = (float)(origin.y + (yStep + (radius + offset))*(-size/2 + row));
                tiles[i] =  new Vector3(xPos, 0, yPos);
                i++;
            }
        }

        return tiles;
    }

  public static Vector3[] HexagonFromPoint(Vector2 origin, float radius)
    {
        Vector3[] points = new Vector3[6];
        float angle30 = Mathf.Deg2Rad*30;
        float angle60 = Mathf.Deg2Rad*60;
        for(int i = 0; i < 6; i++) {
            points[i] = new Vector3(origin.x + Mathf.Cos(angle30 + (angle60*i))*radius, 0, origin.y + Mathf.Sin(angle30 + (angle60*i))*radius);
        }
        return points;
    }

    public static Map GenerateMap(int size, float radius, MapShape shape, TileGeneration generation, int seed)
    {
        Dictionary<string, Location> locations = new Dictionary<string, Location>();
        Dictionary<(int, int), Path> paths = new Dictionary<(int, int), Path>();
        List<Tile> tiles = new List<Tile>();
        int currentLocationId = 0;
        int currentPathId = 0;
        int currentTileId = 0;

        UnityEngine.Random.InitState(seed);

        // Generate tiles with given map shape
        Vector3[] tileLocations = GenerateTilesOfShape(shape, size, radius);

        // Generate all tiles, locations, and paths 
        foreach(Vector3 l in tileLocations)
        {
            // Initialize tile
            Tile tile = new Tile() {
                id = currentTileId,
                position = l,
                value = UnityEngine.Random.Range(2, 10),
            };
            currentTileId++;
            
            Vector3[] surroundingPoints = MapUtil.HexagonFromPoint(new Vector2(l.x, l.z), radius);
            tile.locations = new Location[surroundingPoints.Length];
            Location startLocation = null;
            Location prevLocation = null;
            for(int i = 0; i < surroundingPoints.Length; i++)
            {
                Vector3 point = surroundingPoints[i];
                Location location = null;
                String pointKey = point.x.ToString("0.000") + "_" + point.z.ToString("0.000");
                if(!locations.ContainsKey(pointKey)) {
                    // Locatino does not already exist, add it to the dictionary
                    location = new Location() {
                        id = currentLocationId,
                        position = point,
                        type = LocationType.Available,
                    };
                    locations.Add(pointKey, location);
                    currentLocationId++;
                } else {
                    // Location already exist, extract it from the dictionary
                    location = locations[pointKey];
                }

                // Check if the location is the first location of the tile
                if(startLocation == null) {
                    startLocation = location;
                }
                
                // Add path between the previous location
                if(prevLocation != null) {
                    // Check if there already is an existing path between the locations
                    if(!paths.ContainsKey((location.id, prevLocation.id)) && !paths.ContainsKey((prevLocation.id, location.id)))
                    {
                        paths.Add((location.id, prevLocation.id), new Path() {
                            id = currentPathId,
                            between = new Tuple<Location, Location>(location, prevLocation)
                        });
                        currentPathId++;
                    }
                }
                prevLocation = location;
                tile.locations[i] = location;
            }

            // Add a path between the previous location and the start location
            // Check if there already is an existing path between the locations
            if(!paths.ContainsKey((startLocation.id, prevLocation.id)) && !paths.ContainsKey((prevLocation.id, startLocation.id)))
            {
                paths.Add((startLocation.id, prevLocation.id), new Path() {
                    id = currentPathId,
                    between = new Tuple<Location, Location>(startLocation, prevLocation)
                });
                currentPathId++;
            }
            // Reset the locations
            prevLocation = null;
            startLocation = null;

            tiles.Add(tile);
        }

        // Generate map tile types
        tiles = GenerateTileTypes(generation, tiles, seed);

        // Convert map attributes to 
        Map map = new Map();
        map.AddLocations(locations.Values);
        map.AddTiles(tiles);
        map.AddPaths(paths.Values);
        return map;
    }

    public static Vector3[] GenerateTilesOfShape(MapShape shape, int size, float radius)
    {
        switch(shape)
        {
            case MapShape.HexagonalLattice: {
                return MapUtil.HexagonalLattice(Vector3.zero, size, radius, 0f);
            }
            case MapShape.Doughnut: {
                Vector3[] tiles = MapUtil.HexagonalLattice(Vector3.zero, size, radius, 0f); 
                Vector3 pos = Vector3.zero;
                foreach (Vector3 t in tiles) {
                  pos += t;
                }
                pos /= tiles.Length;
                return tiles
                  .Where(t => Vector3.Distance(t, pos) > 10f)
                  .ToArray();
            }
            default: {
                return null;
            }
        }
    }

    public static List<Tile> GenerateTileTypes(TileGeneration generation, List<Tile> tiles, int seed)
    {
        switch(generation) {
            case TileGeneration.Random: {
                System.Random r = new System.Random(seed);
                int index = 0;
                foreach(Tile t in tiles.OrderBy(t => r.Next(1000000))) {
                    Array values = Enum.GetValues(typeof(TileType));
                    t.type = (TileType)values.GetValue(index++ % values.Length);
                }
                return tiles;
            }

            default: {
                return tiles;
            }
        }
    }

}
