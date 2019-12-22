using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using System;
using System.Linq;

public class GameController : MonoBehaviour, ITurnCallback
{
    public MapController mapController;
    public UIController uiController;
    public IActionHandler handler;

    private PlayerController localPlayer { get; set; }
    private Dictionary<string, PlayerController> players;
    private PlayerController currentPlayer;

    private int VICTORY_POINTS_TO_WIN = 10;

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
        
        players = new Dictionary<string, PlayerController>();

        mapController.GenerateMap();
        mapController.EnableLocationBoxColliders(false);
    }

    public void Update() {
        if(Input.GetMouseButtonDown(0)) {
            HandleMouseClick();
        }
        if(Input.GetKeyDown(KeyCode.Escape)) {
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
            localPlayer.GainResources();
        }

        // Enable turn for new player
        currentPlayer = players[newPlayer];
        if(currentPlayer.IsMine()) {
            currentPlayer.EnableTurn(true);
        }

        Debug.Log($"It is now {currentPlayer.player.name}'s turn.");
        uiController.DisplayEventText($"It's {currentPlayer.player.name}'s turn!");
        uiController.ShowPlayerTurn(newPlayer);
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
            string diplayName = player.id.Equals(localPlayer.player.id) ? "You" : player.name;
            uiController.DisplayGainedResource(diplayName, (ResourceStorage)info.data);
        }
    }

    public void ExchangeResources(ResourceType from, ResourceType to) {
        localPlayer.ExchangeResources(from, to);
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
    #endregion
}
