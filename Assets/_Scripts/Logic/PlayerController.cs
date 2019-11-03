using System;
using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MapController mapController;
    public List<Location> locations;
    public List<Path> paths;
    public List<WorkerController> workers;

    public enum State {
        None,
        WorkerState,
        WorkerSelected,
    }

    [Header("Prefabs")]
    public GameObject workerPrefab;

    [Header("Resources")]
    public int wood = 0;
    public int stone = 0;
    public int cobber = 0;
    public int wheat = 0;
    public int wool = 0;

    public void Start() {
        locations = new List<Location>();
        paths = new List<Path>();
        workers = new List<WorkerController>();
        mapController = GameObject.FindGameObjectWithTag("GameController").GetComponent<MapController>();
    }

    public void CreateWorker(Location location) {
        // Initialize workers
        Worker worker = new Worker() {
            id = Guid.NewGuid(),
            location = location
        };

        // Initialize worker in scene
        GameObject workerObject = GameObject.Instantiate(workerPrefab, location.position, Quaternion.identity);
        WorkerController workerController = workerObject.GetComponent<WorkerController>();
        workerController.Initialize(worker);
        workers.Add(workerController);
    }
}
