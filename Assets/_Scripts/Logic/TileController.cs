using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public Tile tile;
    public Transform prefabHolder;

    [Header("Prefabs")]
    public GameObject forestPrefab;
    public GameObject mountainPrefab;
    public GameObject arablePrefab;

    public void Initialize(Tile newTile, float radius) {
        tile = newTile;

        transform.position = newTile.position;
        transform.rotation = Quaternion.identity;

        GameObject envPrefab = null;
        switch(tile.type) {
            case TileType.Forest: {
                envPrefab = GameObject.Instantiate(forestPrefab, transform.position, Quaternion.identity);
                break;
            }

            case TileType.Mountain: {
                envPrefab = GameObject.Instantiate(mountainPrefab, transform.position, Quaternion.identity);
                break;
            }

            case TileType.Arable: {
                envPrefab = GameObject.Instantiate(arablePrefab, transform.position, Quaternion.identity);
                break;
            }
        }
        envPrefab.transform.SetParent(prefabHolder);
        envPrefab.transform.localRotation = Quaternion.Euler(0, 30, 0);
        envPrefab.transform.localPosition = Vector3.zero;
        envPrefab.transform.localScale = new Vector3(radius, 3, radius);
    } 

}
