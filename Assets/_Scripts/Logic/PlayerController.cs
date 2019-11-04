using System;
using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private MapController mapController;
    public enum State {
        None,
        WaitForTurn,
        WorkerMovement,
    }

    [Header("Player Belongings")]
    public List<Location> locations;
    public List<Path> paths;
    public List<WorkerController> workers;

    [Header("State")]
    public State state = State.None;

    [Header("Prefabs")]
    public GameObject workerPrefab;

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

    public void MoveWorker(WorkerController worker, Location location) {
        worker.MoveWorker(location);
    }

    public void SetState(State newState) {
        if(newState == State.WaitForTurn)
        {
            
        }
        state = newState;
    }

    public void EnableWorkers(bool enable)
    {
        foreach(WorkerController w in workers)
        {
            w.enabled = enable;
        }
    }
}
