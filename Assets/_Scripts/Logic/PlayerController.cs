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
        PathPlacement,
        HousePlacement,
    }

    [Header("Player Belongings")]
    public List<Location> locations;
    public List<Path> paths;
    public List<WorkerController> workers;

    [Header("State")]
    public State state = State.None;

    [Header("GameObject")]
    public GameObject workerPrefab;
    public GameObject pathTargetPrefab;

    [Header("Placement variables")]
    private PathController[] selectablePaths;
    private GameObject pathTarget;
    
    
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
        // CLEAN PREVIOUS STATE
        if(state == State.PathPlacement)
        {
            pathTarget.SetActive(false);
            selectablePaths = null;
        }

        // NEW STATE LOGIC
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

    public void StartHousePlacement(WorkerController worker) {

    }

    public void StartPathPlacement(WorkerController worker, PathController[] pathControllers) {
        
        selectablePaths = pathControllers;

        // Instatiate path target holder
        if(pathTarget == null)
        {
            pathTarget = GameObject.Instantiate(pathTargetPrefab, Vector3.zero, Quaternion.identity);
        }
        pathTarget.SetActive(true);

        // Show selectable indicator
        foreach(var pathC in selectablePaths)
        {
            pathC.SetSelectable(true);
        }

        // Change state
        SetState(State.PathPlacement);
    }

    public void Update()
    {
        if(state == State.PathPlacement)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pathTarget.transform.position = new Vector3(mousePos.x, 0, mousePos.z);
        }
    }
}
