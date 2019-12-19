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
    public GameObject cityPrefab;

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

        location.occupiedBy = player.id;
        location.type = LocationType.House;
        
        // Instantiate house object
        GameObject houseObject = GameObject.Instantiate(housePrefab, transform.position, Quaternion.Euler(-90, 0, 0));
        houseObject.transform.SetParent(structureHolder.transform);
        houseObject.transform.localPosition = Vector3.zero;
        houseObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", player.GetColor());
    }

    public void BuildCity(Player player)
    {
        if(location.type != LocationType.House)
        {
            return;
        }

        location.occupiedBy = player.id;
        location.type = LocationType.City;

        // Remove existing house if exists
        foreach(Transform child in structureHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Instatiate city object
        GameObject cityObject = GameObject.Instantiate(cityPrefab, transform.position, Quaternion.identity);
        cityObject.transform.SetParent(structureHolder.transform);
        cityObject.transform.localPosition = Vector3.zero;
        cityObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", player.GetColor());
    }
}
