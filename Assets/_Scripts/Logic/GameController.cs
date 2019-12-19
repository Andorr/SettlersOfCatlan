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

        // Enable turn for new player
        currentPlayer = players[newPlayer];
        if(currentPlayer.IsMine()) {
            currentPlayer.EnableTurn(true);
        }

        Debug.Log($"It is now {currentPlayer.player.name}'s turn.");
        uiController.DisplayText($"It's {currentPlayer.player.name} turn!");
        uiController.ShowPlayerTurn(newPlayer);
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
