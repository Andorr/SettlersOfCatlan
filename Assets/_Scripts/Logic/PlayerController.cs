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
        ThiefMovement,
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
        if(enable) {
            SetState(State.None);
            
           
            if(gameController.state == GameController.GameState.PlayersCreateHouses) {
                mapController.EnableLocationBoxColliders(true);
            } else {
                uiController.EnableEndTurnButton(true, () => EndTurn());
                uiController.EnableSideActionPanel(true);
                EnableWorkers(enable);
            }
        } else {
            SetState(State.WaitForTurn);
            uiController.EnableEndTurnButton(false, null);
            mapController.EnableLocationBoxColliders(false);
            uiController.EnableSideActionPanel(false);
        }
    }

    public void SetState(State newState)
    {
        if (newState == State.ThiefMovement) {
            foreach(var tileController in mapController.GetAllTileControllers()) {
                tileController.SetSelectable(true);
            }
            uiController.EnableEndTurnButton(false, null);
            uiController.EnableSideActionPanel(false);
            uiController.EnableActionPanel(false);
            mapController.EnableLocationBoxColliders(false);
            EnableWorkers(false);
        } else if (state == State.ThiefMovement && newState != State.WaitForTurn) {
            uiController.EnableEndTurnButton(true, () => EndTurn());
            uiController.EnableSideActionPanel(true);
            mapController.EnableLocationBoxColliders(true);
            EnableWorkers(true);
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

    public void GainResources(int thiefTileId) {
        // Let the player gain resources
        (int wood, int stone, int clay, int wheat, int wool) = mapController.CalculateGainableResources(player, thiefTileId);
        AddResources(wood, stone, clay, wheat, wool);
        
        if(photonView.IsMine) {
            RaiseEvent(ActionType.GainedResources, new ResourceStorage {
                wood = wood,
                stone = stone,
                clay = clay,
                wheat = wheat,
                wool = wool,
            });
        }
    }

    public void CannotStealResources(String stealeeName) {
        if (photonView.IsMine) {
            RaiseEvent(ActionType.CannotStealResouce);
        }
    }

    public void StealResourceFromPlayer(Player stealee) {
        var rnd = new System.Random();
        var from = new ResourceStorage();
        var to = new ResourceStorage();
        var namesCount = Enum.GetNames(typeof(ResourceType)).Length;
        ResourceType type = (ResourceType) rnd.Next(0, namesCount);
        to.AddResource(type, 1);
        gameController.ExecuteTrade(stealee, from, to);
        RaiseEvent(ActionType.ThiefStoleResource, new object[]{type, PhotonNetwork.LocalPlayer.NickName, stealee.name});
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

    public void MoveThief(int newTileId) {
        if (photonView.IsMine) {
            photonView.RPC("OnMoveThief", RpcTarget.All, newTileId);
        }

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

    public void AddResources(ResourceStorage storage, bool shouldBroadcast = false) {
        AddResources(storage.wood, storage.stone, storage.clay, storage.wheat, storage.wool, shouldBroadcast);
    }

    public void AddResources(int wood, int stone, int clay, int wheat, int wool, bool shouldBroadcast = false) {
        var resourceStore = player.resources;
        resourceStore.wood += wood;
        resourceStore.stone += stone;
        resourceStore.clay += clay;
        resourceStore.wheat += wheat;
        resourceStore.wool += wool;

        if(photonView.IsMine || shouldBroadcast) {
            BroadcastResourceChange();
        }

        uiController.UpdatePlayerUI(player);
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
        Card card = player.cards[id];
        if(card == null) {
            return;
        }
        card.UseCard();
        
        if(photonView.IsMine) {
            BroadcastCardUsage(id);
            BroadcastResourceChange();

            // Do something...
            switch(card.cardType) {
                case CardType.Thief: {
                    // TODO: Start thief placement mode
                    SetState(State.ThiefMovement);
                    break;
                }
                case CardType.VP: {
                    break;
                }
            }
        }

        RaiseEvent(ActionType.UseCard, card);
    }

    public void RetriveCard(CardType cardType, string id = null){
        string g = id == null ? Guid.NewGuid().ToString() : id;
        Card card = new Card(g.ToString(),cardType);

        ResourceUtil.PurchaseCard(player.resources);
        
        player.cards.Add(g,card);

        RaiseEvent(ActionType.BuyCard, card);

        if(photonView.IsMine){
            BrodcastCardRetrieved(g, (int)card.cardType);
        }
    }
    private void BroadcastCardUsage(string id){
        photonView.RPC("OnCardUsed", RpcTarget.Others, id);
    }
    private void BrodcastCardRetrieved(string id, int type){
        photonView.RPC("OnCardRetrieved", RpcTarget.Others, id, type);
    }

    public void BroadcastEvent(string message) {
        photonView.RPC("OnEventBroadcasted", RpcTarget.All, message);
    }

    public void SendTradeRequest(Player playerToTradeWith, ResourceStorage from, ResourceStorage to) {
        // Should not be possible to trade with yourself
        if(playerToTradeWith.id.Equals(player.id)) {
            return;
        }

        // Send trade request
        photonView.RPC("OnTradeRequestReceived", RpcTarget.All, playerToTradeWith.id, from, to);
    }

    public void SendTradeRequestAnswer(bool answer, Player playerToTradeWith, ResourceStorage from, ResourceStorage to) {
        // Should not be possible to trade with yourself
        if(playerToTradeWith.id.Equals(player.id)) {
            return;
        }

        photonView.RPC("OnTradeRequestAnswerReceived", RpcTarget.All,  playerToTradeWith.id, answer, from, to);
    }

    public void CancelTradeRequest() {
        photonView.RPC("OnTradeRequestCancelled", RpcTarget.All);
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
    public void OnMoveThief(int newTileId) {
        if (gameController.thiefTileId != 0) {
            TileController oldTile = mapController.GetTileControllerById(gameController.thiefTileId);
            oldTile.RemoveThief(gameController);
        }
        TileController newTile = mapController.GetTileControllerById(newTileId);
        newTile.AddThief(gameController);
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

    [PunRPC]
    void OnTradeRequestReceived(string playerIDToTradeWith, ResourceStorage to, ResourceStorage from) {
        if(photonView.IsMine) {
            gameController.OnTradeRequested(playerIDToTradeWith, from, to);
        }
    }

    [PunRPC]
    void OnTradeRequestAnswerReceived(string playerIDToTradeWith, bool answer, ResourceStorage to, ResourceStorage from) {
        if(photonView.IsMine) {
            gameController.OnTradeRequestAnswered(answer, playerIDToTradeWith, from, to);
        }
    }

    [PunRPC]
    void OnTradeRequestCancelled() {
        if(photonView.IsMine) {
            gameController.OnTradeRequestCancelled();
        }
    }

    [PunRPC]
    void OnCardUsed(string id) {
        UseCard(id);
    }

    [PunRPC]
    void OnCardRetrieved(string cardId, int cardType) {
        CardType type = (CardType)cardType;
        RetriveCard(type, cardId);
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
