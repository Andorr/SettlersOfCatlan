using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using UnityEngine.UI;

public class TileController : MonoBehaviour
{
    [SerializeField]
    public Tile tile;
    public Transform prefabHolder;
    public Text probabilityText;

    [Header("Prefabs")]
    public GameObject forestPrefab;
    public GameObject mountainPrefab;
    public GameObject fieldPrefab;
    public GameObject pasturePrefab;
    public GameObject hillPrefab;


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

            case TileType.Field: {
                envPrefab = GameObject.Instantiate(fieldPrefab, transform.position, Quaternion.identity);
                break;
            }

            case TileType.Pasture: {
                envPrefab = GameObject.Instantiate(pasturePrefab, transform.position, Quaternion.identity);
                break;
            }

            case TileType.Hill: {
                envPrefab = GameObject.Instantiate(hillPrefab, transform.position, Quaternion.identity);
                break;
            }
        }
        envPrefab.transform.SetParent(prefabHolder);
        envPrefab.transform.localRotation = Quaternion.Euler(0, 30, 0);
        envPrefab.transform.localPosition = Vector3.zero;
        envPrefab.transform.localScale = new Vector3(radius, 3, radius);

        // Set probability text
        if(probabilityText != null) {
            probabilityText.text = tile.probability.ToString();
        }
    } 

}
