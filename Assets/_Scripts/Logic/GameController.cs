using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour, ITurnCallback
{
    public MapController mapController;
    public UIController uiController;
    public AudioController audioController;

    private IActionHandler handler;
    public bool enableHandlers = false;
    public bool disableEsc = false;

    private PlayerController localPlayer { get; set; }
    private Dictionary<string, PlayerController> players;
    private PlayerController currentPlayer;

    private int VICTORY_POINTS_TO_WIN = 10;

    public int thiefTileId = -1;

    public enum GameState {
        PlayersCreateHouses,
        Play,
        End,
    }

    public GameState state = GameState.PlayersCreateHouses;

    # region Unity Methods
    public void Start() {
        mapController = GetComponent<MapController>();
        uiController = GetComponent<UIController>();
        audioController = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>();
        
        players = new Dictionary<string, PlayerController>();

        mapController.GenerateMap();
        mapController.EnableLocationBoxColliders(false);
    }

    public void Update() {
        if(uiController.IsUIPanelsOpen() || !enableHandlers) {
            return;
        }

        if(Input.GetMouseButtonDown(0)) {
            HandleMouseClick();
        }
        if(Input.GetKeyDown(KeyCode.Escape) && !disableEsc) {
            UnselectHandler();
        }
        HandleMouseHover();
    }
    # endregion

    # region Game Logic
    public PlayerController GetLocalPlayer() {
        return localPlayer;
    }

    public void SetLocalPlayer(PlayerController pc) {
        this.localPlayer = pc;
    }

    public void AddPlayer(PlayerController pc) {
        players.Add(pc.player.id, pc);
    }

    public void ChangeState(GameState newState)
    {
        if(state == GameState.PlayersCreateHouses && newState == GameState.Play)
        {
            mapController.EnableLocationBoxColliders(false);
        }
        state = newState;
    }

    public void NewTurn(string newPlayer)
    {
        UnselectHandler();
        if(state == GameState.PlayersCreateHouses) {
            // Check if all players have created two houses and a worker, if so change state. Maybe let this logic be a part of the master client?
            if (players.Values.Where(p => p.locations.Count == 2 && p.workers.Count == 2).Count() == players.Count) {
                ChangeState(GameState.Play);
            }
        }

        // Gain resources for the round
        if(GameState.Play == state) {
            localPlayer.GainResources(thiefTileId);
        }

        // Enable turn for new player
        currentPlayer = players[newPlayer];
        if(currentPlayer.IsMine()) {
            currentPlayer.EnableTurn(true);
            enableHandlers = true;
        } else {
            enableHandlers = false;
        }
        disableEsc = false;

        Debug.Log($"It is now {currentPlayer.player.name}'s turn.");
        uiController.DisplayEventText($"It's {currentPlayer.player.name}'s turn!");
        uiController.ShowPlayerTurn(newPlayer);

        // Play sound effect
        audioController.PlayClip("Sounds/NewTurn");
    }

    public void ActionCallback(ActionInfo info) {
        var player = info.player;

        // Update the player's victory points
        player.victoryPoints = mapController.CalculateVictoryPoints(player);
        uiController.UpdatePlayerUI(player);

        // Log the action
        // TODO: Log action to the "Chat-log"
        Debug.Log(info);

        // Check if the player has won the game or not
        if(Photon.Pun.PhotonNetwork.IsMasterClient) {
            if(player.victoryPoints >= VICTORY_POINTS_TO_WIN) {
                // TODO: Communicate to the other players that the player has won and the game is over.
                ChangeState(GameState.End);
                players[player.id].BroadcastVictory();
            }
        }

        // Display resources text if the player gained resources
        if(info.actionType == ActionType.GainedResources) {
            string displayName = player.id.Equals(localPlayer.player.id) ? "You" : player.name;
            uiController.DisplayGainedResource(displayName, (ResourceStorage)info.data);
        }
        else if(info.actionType == ActionType.UseCard) {
            string displayName = player.id.Equals(localPlayer.player.id) ? "You" : player.name;
            uiController.DisplayUsedCard(displayName, (Card)info.data);
            audioController.PlayClip("Sounds/Card", 0.4f);
        }
        else if(info.actionType == ActionType.BuyCard && IsLocalPlayer(player)) {
            uiController.DisplayUsedCard("You", (Card)info.data, "You earned:");
            audioController.PlayClip("Sounds/Card", 0.4f);
        }
        else if(info.actionType == ActionType.ThiefStoleResource) {
            uiController.DisplayEventText(info.ToString(), 4f);
            uiController.DisplayEventImage(uiController.knight, 4f);
            audioController.PlayClip("Sounds/Gain", 0.3f);
        }
        else if(info.actionType == ActionType.CannotStealResouce && IsLocalPlayer(player)) {
            uiController.DisplayEventText("There is no resources to steal.", 3f);
        }
    }

    public void EndGame(Player winner) {
        ChangeState(GameState.End);

        // Deactivate all actions
        UnselectHandler();
        foreach(PlayerController player in players.Values) {
            player.EnableTurn(false);
        }
        mapController.EnableLocationBoxColliders(false);

        // Show win screen
        uiController.EnableWinPanel(true, winner);
    }

    public Player GetPlayers(out Player[] players) {
        players = this.players.Values.Where(p => !p.player.id.Equals(localPlayer.player.id)).Select(pc => pc.player).ToArray();
        return localPlayer.player;
    }

    public Player GetPlayerById(string playerId) {
        return players[playerId].player;
    }

    public bool IsLocalPlayer(Player player) {
        return localPlayer.player.id.Equals(player.id);
    }

    # endregion

    # region TradeLogic
    public void ExecuteTrade(Player playerToTradeWith, ResourceStorage from, ResourceStorage to) {
        var fromNegation = ResourceUtil.Negation(from);
        var toNegation = ResourceUtil.Negation(to);

        localPlayer.AddResources(fromNegation + to); // Remove "from", add "to"
        players[playerToTradeWith.id].AddResources(from + toNegation, true); // Add "from", remove "to"
    }

    public void ExchangeResources(ResourceType from, ResourceType to) {
        localPlayer.ExchangeResources(from, to);
    }

    public void SendTradeRequest(Player playerToTradeWith, ResourceStorage from, ResourceStorage to) {
        players[playerToTradeWith.id].SendTradeRequest(localPlayer.player, from, to);
    }

    public void OnTradeRequested(string playerIDToTradeWith, ResourceStorage from, ResourceStorage to) {
        PlayerController playerToTradeWith = players[playerIDToTradeWith];
        uiController.ShowTradeRequest(playerToTradeWith.player, from, to);
    }

    public void SendTradeRequestAnswer(bool accepted, Player playerToTradeWith, ResourceStorage from, ResourceStorage to) { 
        players[playerToTradeWith.id].SendTradeRequestAnswer(accepted, localPlayer.player, from, to);
        uiController.DisableTrading();
    }

    public void OnTradeRequestAnswered(bool accepted, string playerIDToTradeWith, ResourceStorage from, ResourceStorage to) {
        PlayerController playerToTradeWith = players[playerIDToTradeWith];
        if(accepted) {
            // Send request
            ExecuteTrade(playerToTradeWith.player, from, to);
            uiController.DisableTrading();
            uiController.DisplayEventText("Trade accepted!", 4f);
        } else {
            // TODO: Fix better feedback for when the trade request was declined.
            uiController.DisableTrading();
            uiController.DisplayEventText("Trade declined!", 4f);
        }
    }

    public void SendTradeRequestCancellation(Player player) {
        players[player.id].CancelTradeRequest();
    }

    public void OnTradeRequestCancelled() {
        uiController.DisableTrading();
    }
    # endregion

    # region Handler Logic
    public void HandleMouseClick() {
        RaycastHit hit;
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(r, out hit)) {
            var actionHandler = hit.collider.gameObject.GetComponent<IActionHandler>();
            if(actionHandler == null) {
                return;
            }

            bool shouldOverride = false;
            if(handler != null) {
                
                // Check for override
                if(handler.TryOverride)
                {
                    shouldOverride = handler.OnTryOverride(this, hit.collider.gameObject);
                }

                handler.OnUnselected(this);
            }
            if(!shouldOverride)
            {
                handler = actionHandler;
                handler.OnSelected(this);
            }
        }
    }
    public void HandleMouseHover() {
        RaycastHit hit;
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(r, out hit)) {
            var actionHandler = hit.collider.gameObject.GetComponent<IActionHandler>();
            if(actionHandler == null) {
                return;
            }
   
            actionHandler.OnHover(this);
        }
    }

    public void UnselectHandler() {
        if(handler != null) {
            handler.OnUnselected(this);
            handler = null;
        }
    }

    // Temp function, should be removed... plays thief card for localplayer
    public void PlayThief() {
        foreach(var tileController in mapController.GetAllTileControllers()) {
            tileController.SetSelectable(true);
        }
    }

    public void SetThiefTile(int newTileId) {
        thiefTileId = newTileId;
    }

    public TileController GetThiefTile() {
        return thiefTileId != -1 ? mapController.GetTileControllerById((int) thiefTileId) : null;
    }

    #endregion
}
