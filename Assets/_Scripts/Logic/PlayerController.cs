using System;
using System.Linq;
using System.Collections.Generic;
using Photon.Pun;
using State;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback, IPlayerAction
{
    // Controllers
    private GameController gameController;
    private UIController uiController;
    private MapController mapController;
    private TurnManager turnManager;

    public delegate void ActionCallback(ActionInfo info);
    public event ActionCallback onActionEvent;

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
    
    
    public void Awake()
    {
        locations = new List<Location>();
        paths = new List<Path>();
        workers = new List<WorkerController>();
        player.cards = new Dictionary<string, Card>();

        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        mapController = gameController.mapController;
        uiController = gameController.uiController;
        turnManager = GetComponent<TurnManager>();
    }

    public void Initialize(Player player)
    {
        this.player = player;
    }

    public bool IsMine()
    {
        return photonView.IsMine;
    }

    public void EnableTurn(bool enable)
    {
        EnableWorkers(enable);
        if(enable) {
            SetState(State.None);
            uiController.EnableEndTurnButton(true, () => EndTurn());
           
            if(gameController.state == GameController.GameState.PlayersCreateHouses) {
                mapController.EnableLocationBoxColliders(true);
            } else {
                uiController.EnableSideActionPanel(true);
            }

            // Let the player gain resources
            (int wood, int stone, int clay, int wheat, int wool) = mapController.CalculateGainableResources(player);
            AddResources(wood, stone, clay, wheat, wool);
            RaiseEvent(ActionType.GainedResources, new ResourceStorage {
                wood = wood,
                stone = stone,
                clay = clay,
                wheat = wheat,
                wool = wool,
            });

        } else {
            SetState(State.WaitForTurn);
            uiController.EnableEndTurnButton(false, null);
            mapController.EnableLocationBoxColliders(false);
             uiController.EnableSideActionPanel(false);
        }
    }

    public void SetState(State newState)
    {
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

    public void BroadcastVictory()
    {
        // Broadcasts to all the players that this player has won the game.
        photonView.RPC("OnPlayerWon", RpcTarget.AllBufferedViaServer);
    }

    private void RaiseEvent(ActionType type, object data = null) {
        if(onActionEvent != null) {
            var info = ActionInfo.New(type, player);
            info.data = data;
            onActionEvent(info);
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
        mapController.AddWorkerToMap(worker);

        if(photonView.IsMine) {
            photonView.RPC("OnWorkerCreated", RpcTarget.Others, location.id, worker.id.ToString());
        }

        // Raise event
        RaiseEvent(ActionType.CreateWorker);
        
        return workerController;
    }

    public void MoveWorker(Worker worker, Location location) {
        var wc = workers.Where(w => w.worker.id.Equals(worker.id)).FirstOrDefault();
        if(wc == null) {
            return;
        }

        wc.MoveWorker(location);

        if(photonView.IsMine) {
            photonView.RPC("OnMoveWorker", RpcTarget.Others, worker.id, location.id);
            BroadcastResourceChange();
        }

        // Raise event
        RaiseEvent(ActionType.MoveWorker);
    }

    public void BuildPath(Path path)
    {   
        var exists = mapController.GetPathController(path, out var pc);
        if(!exists) {
            return;
        }

        // Calculate the rotation of the object
        pc.BuildPath(player);
        ResourceUtil.PurchasePath(player.resources);

        paths.Add(pc.path);

        if(photonView.IsMine) {
            photonView.RPC("OnPathCreated", RpcTarget.Others, path.id);
            BroadcastResourceChange();
        }

        // Raise event
        RaiseEvent(ActionType.BuildPath);
    }

    public void BuildHouse(Location location)
    {
        var exists = mapController.GetLocationController(location, out var lc);
        if(!exists) {
            return;
        }

        lc.BuildHouse(player);
        ResourceUtil.PurchaseHouse(player.resources);

        locations.Add(lc.location);

        if(photonView.IsMine) {
            photonView.RPC("OnHouseCreated", RpcTarget.Others, location.id);
            BroadcastResourceChange();
        }

        // Raise event
        RaiseEvent(ActionType.BuildHouse);
    }

    public void BuildCity(Location location)
    {
        var exists = mapController.GetLocationController(location, out var lc);
        if(!exists) {
            return;
        }

        lc.BuildCity(player);
        ResourceUtil.PurchaseCity(player.resources);

        if(photonView.IsMine) {
            photonView.RPC("OnCityCreated", RpcTarget.Others, location.id);
            BroadcastResourceChange();
        }

        // Raise event
        RaiseEvent(ActionType.BuildCity);
    }

    public void EndTurn()
    {
        EnableTurn(false);
        turnManager.EndTurn();

        // Raise event
        RaiseEvent(ActionType.EndTurn);
    }

    public void AddResources(int wood, int stone, int clay, int wheat, int wool) {
        var resourceStore = player.resources;
        resourceStore.wood += wood;
        resourceStore.stone += stone;
        resourceStore.clay += clay;
        resourceStore.wheat += wheat;
        resourceStore.wool += wool;

        if(photonView.IsMine) {
            BroadcastResourceChange();
        }
    }

    public void ExchangeResources(ResourceType from, ResourceType to)
    {
        player.resources.AddResource(from, -3);
        player.resources.AddResource(to, 1);
        
        if(photonView.IsMine) {
            BroadcastResourceChange();
        }

        RaiseEvent(ActionType.ExchangedResources, new ResourceType[]{from, to});
    }

    private void BroadcastResourceChange() {
        uiController.UpdatePlayerUI(player);
        var resourceStore = player.resources;
        photonView.RPC("OnResourcesChanged", RpcTarget.Others, resourceStore.wood, resourceStore.stone, resourceStore.clay, resourceStore.wheat, resourceStore.wool);
    }

    public void UseCard(string id){
        player.cards[id].UseCard();

        if(photonView.IsMine) {
            BroadcastCardUsage(id);
        }
        
    }

    public void RetriveCard(CardType cardType){
        string g = Guid.NewGuid().ToString();
        Card card = new Card(g.ToString(),cardType);
        player.cards.Add(g,card);

        if(photonView.IsMine) {
            BrodcastCardRetrived(g, (int)card.cardType);
        }

    }
    private void BroadcastCardUsage(string id){
        photonView.RPC("OnCardUsage", RpcTarget.Others, id);
    }
    private void BrodcastCardRetrived(string id, int type){
        photonView.RPC("OnCardRetrived", RpcTarget.Others, id, type);
    }

    public void BroadcastEvent(string message) {
        photonView.RPC("OnEventBroadcasted", RpcTarget.All, message);
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
        MoveWorker(wc.worker, location);
    }

    [PunRPC]
    void OnHouseCreated(int locationId) {
        Location location = mapController.GetLocationById(locationId);
        if(location == null) {
            return;
        }
        BuildHouse(location);
    }

    [PunRPC]
    void OnCityCreated(int locationId) {
        Location location = mapController.GetLocationById(locationId);
        if(location == null) {
            return;
        }
        BuildCity(location);
    }

    [PunRPC]
    void OnPathCreated(int pathId) {
        Path path = mapController.GetPathById(pathId);
        if(path == null) {
            return;
        }
        BuildPath(path);
    }

    [PunRPC]
    void OnResourcesChanged(int wood, int stone, int clay, int wheat, int wool) {
        var resourceStore = player.resources;
        resourceStore.wood = wood;
        resourceStore.stone = stone;
        resourceStore.clay = clay;
        resourceStore.wheat = wheat;
        resourceStore.wool = wool;
        uiController.UpdatePlayerUI(player);
    }

    [PunRPC]
    void OnPlayerWon() {
        gameController.EndGame(player);
    }

    [PunRPC]
    void OnEventBroadcasted(string message) {
        uiController.DisplayEventText(message, 3);
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
            resources = new ResourceStorage(),
        };
        Initialize(gamePlayer);
        gameController.AddPlayer(this);
        mapController.AddPlayerToMap(gamePlayer); // Add the player to the map game state
        uiController.AddPlayer(gamePlayer, photonPlayer.NickName); // Add the player to the UI
        onActionEvent += gameController.ActionCallback;

        if(photonView.IsMine) {
            gameController.SetLocalPlayer(this);
        }
    }


    #endregion
}
