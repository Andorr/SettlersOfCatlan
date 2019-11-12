using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public class LocationController : MonoBehaviour
{
    [SerializeField]
    public Location location;
    public Transform prefabHolder;
    public GameObject selectableIndicator;

    [Header("Prefabs")]
    public GameObject structureHolder;
    public GameObject housePrefab;

    private bool isSelectable = false;

    public void Initialize(Location newLocation, float radius)
    {
        location = newLocation;

        transform.position = location.position;
        transform.GetComponent<BoxCollider>().size = new Vector3(radius/2, 1.5f, radius/2);
    }

    public void SetSelectable(bool selectable) {
        isSelectable = selectable;
        transform.GetComponent<BoxCollider>().enabled = selectable;
        selectableIndicator.SetActive(selectable);
    }

    public void BuildHouse(Player player)
    {
        if(location.type != LocationType.Available)
        {  
            return;
        }

        location.occupiedBy = player;
        location.type = LocationType.House;
        
        // Instantiate path object
        GameObject houseObject = GameObject.Instantiate(housePrefab, transform.position, Quaternion.Euler(-90, 0, 0));
        houseObject.transform.SetParent(structureHolder.transform);
        houseObject.transform.localPosition = Vector3.zero;
        houseObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", player.GetColor());
    }
}
