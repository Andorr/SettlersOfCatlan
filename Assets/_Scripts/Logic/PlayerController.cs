using System;
using System.Linq;
using System.Collections.Generic;
using Photon.Pun;
using State;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPlayerAction
{
    private GameController gameController;
    private UIController uiController;
    private MapController mapController;
    private TurnManager turnManager;

    public enum State {
        None,
        WaitForTurn,
        WorkerMovement,
        PathPlacement,
        HousePlacement,
    }

    [Header("Player Belongings")]
    public Player player;
    public List<Location> locations;
    public List<Path> paths;
    public List<WorkerController> workers;

    [Header("State")]
    public State state = State.WaitForTurn;

    [Header("GameObject")]
    public GameObject workerPrefab;
    
    
    public void Awake() {
        locations = new List<Location>();
        paths = new List<Path>();
        workers = new List<WorkerController>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        mapController = gameController.mapController;
        uiController = gameController.uiController;
        turnManager = GetComponent<TurnManager>();
    }

    public void Initialize(Player player)
    {
        this.player = player;
    }

    public void EnableTurn(bool enable)
    {
        if(!photonView.IsMine) {
            return;
        }

        EnableWorkers(enable);
        if(enable) {
            SetState(State.None);
            uiController.EnableEndTurnButton(true, () => EndTurn());
           
            if(gameController.state == GameController.GameState.PlayersCreateHouses) {
                mapController.EnableLocationBoxColliders(true);
            }

            // Let the player gain resources
            (int wood, int stone, int clay, int wheat, int wool) = mapController.CalculateGainableResources(player);
            AddResources(wood, stone, clay, wheat, wool);

        } else {
            SetState(State.WaitForTurn);
            uiController.EnableEndTurnButton(false, null);
            mapController.EnableLocationBoxColliders(false);
        }
    }

    public void SetState(State newState) {
        // CLEAN PREVIOUS STATE
        

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
            w.EnableWorker(enable);
        }
    }

    # region Player Actions
    public WorkerController CreateWorker(Location location) {
        // Initialize workers
        Worker worker = new Worker() {
            id = Guid.NewGuid().ToString(),
            location = location,
            belongsTo = player.id,
        };

        // Initialize worker in scene
        GameObject workerObject = GameObject.Instantiate(workerPrefab, location.position, Quaternion.identity);
        WorkerController workerController = workerObject.GetComponent<WorkerController>();
        workerController.Initialize(worker);
        workers.Add(workerController);
        workerController.EnableWorker(photonView.IsMine);

        if(photonView.IsMine) {
            photonView.RPC("OnWorkerCreated", RpcTarget.Others, location.id, worker.id.ToString());
        }

        return workerController;
    }

    public void MoveWorker(WorkerController worker, Location location) {
        worker.MoveWorker(location);

        if(photonView.IsMine) {
            photonView.RPC("OnMoveWorker", RpcTarget.Others, worker.worker.id, location.id);
            BroadcastResourceChange();
        }
    }

    public void BuildPath(PathController controller)
    {   
        // Calculate the rotation of the object
        controller.BuildPath(player);
        ResourceUtil.PurchasePath(player);

        paths.Add(controller.path);

        if(photonView.IsMine) {
            photonView.RPC("OnPathCreated", RpcTarget.Others, controller.path.id);
            BroadcastResourceChange();
        }
    }

    public void BuildHouse(LocationController controller)
    {
        controller.BuildHouse(player);
        ResourceUtil.PurchaseHouse(player);

        locations.Add(controller.location);

        if(photonView.IsMine) {
            photonView.RPC("OnHouseCreated", RpcTarget.Others, controller.location.id);
            BroadcastResourceChange();
        }
    }

    public void BuildCity(LocationController controller)
    {
        controller.BuildCity(player);
        ResourceUtil.PurchaseCity(player);

        if(photonView.IsMine) {
            photonView.RPC("OnCityCreated", RpcTarget.Others, controller.location.id);
            BroadcastResourceChange();
        }
    }

    public void EndTurn()
    {
        EnableTurn(false);
        turnManager.EndTurn();
    }

    public void AddResources(int wood, int stone, int clay, int wheat, int wool) {
        player.wood += wood;
        player.stone += stone;
        player.clay += clay;
        player.wheat += wheat;
        player.wool += wool;

        uiController.UpdatePlayerUI(player);

        if(photonView.IsMine) {
            BroadcastResourceChange();
        }
    }

    private void BroadcastResourceChange() {
        photonView.RPC("OnResourcesChanged", RpcTarget.Others, player.wood, player.stone, player.clay, player.wheat, player.wool);
    }
    # endregion

    # region RPC Methods
    [PunRPC]
    void OnWorkerCreated(int locationId, string workerId) {
        Location location = mapController.GetLocationById(locationId);
        var workerController = CreateWorker(location);
        workerController.worker.id = workerId; // Set the worker id
    }

    [PunRPC]
    void OnMoveWorker(string workerId, int locationId) {
        Location location = mapController.GetLocationById(locationId);
        WorkerController wc = workers.Where((c) => c.worker.id.Equals(workerId)).FirstOrDefault();
        if(wc == null) {
            Debug.LogError("The worker by id " + workerId + " does not exist!");
            return;
        }
        MoveWorker(wc, location);
    }

    [PunRPC]
    void OnHouseCreated(int locationId) {
        Location location = mapController.GetLocationById(locationId);
        var exists = mapController.GetLocationController(location, out var lc);
        if(!exists) {
            Debug.LogError("The location by id " + locationId + " does not exist!");
            return;
        }
        BuildHouse(lc);
    }

    [PunRPC]
    void OnCityCreated(int locationId) {
        Location location = mapController.GetLocationById(locationId);
        var exists = mapController.GetLocationController(location, out var lc);
        if(!exists) {
            Debug.LogError("The location by id " + locationId + " does not exist!");
            return;
        }
        BuildCity(lc);
    }

    [PunRPC]
    void OnPathCreated(int pathId) {
        Path path = mapController.GetPathById(pathId);
        var exists = mapController.GetPathController(path, out var pc);
        if(!exists) {
            Debug.LogError("The path by id " + pathId + " does not exist!");
            return;
        } 
        BuildPath(pc);
    }

    [PunRPC]
    void OnResourcesChanged(int wood, int stone, int clay, int wheat, int wool) {
        player.wood = wood;
        player.stone = stone;
        player.clay = clay;
        player.wheat = wheat;
        player.wool = wool;
        uiController.UpdatePlayerUI(player);
    }
    # endregion

    # region Photon-Callbacks
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        var photonPlayer = info.Sender;

        // Initialize player color
        var color = photonPlayer.CustomProperties["Color"];
        PlayerColor playerColor = PlayerColor.Blue;
        if(color != null) {
            playerColor = (PlayerColor)color;
        }

        // Initialize player object
        Player gamePlayer = new Player{
            id = photonPlayer.UserId ?? Guid.NewGuid().ToString(),
            color = playerColor,
            name = photonPlayer.NickName,
        };
        Initialize(gamePlayer);
        uiController.AddPlayer(gamePlayer, photonPlayer.NickName);
        gameController.AddPlayer(this);

        if(photonView.IsMine) {
            gameController.SetLocalPlayer(this);
        }
    }
    #endregion
}
