using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    public MapController mapController;
    public UIController uiController;
    public IActionHandler handler;

    public PlayerController localPlayer { get; private set; }
    private PlayerController currentPlayer;
    public enum GameState {
        PlayersCreateHouses,
        Play,
        End,
    }

    public GameState state = GameState.PlayersCreateHouses;

    private PlayerController[] players;

    # region Unity Methods
    public void Start() {
        mapController = GetComponent<MapController>();
        uiController = GetComponent<UIController>();

        mapController.GenerateMap();
        mapController.EnableLocationBoxColliders(true);

        // TODO: When on multiplayer, initiate player objects and set this to PhotonView.isMine
        // localPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        // players = new PlayerController[] {
        //     localPlayer,
        // };
        // for(int i = 0; i < players.Length; i++)
        // {
        //     players[i].Initialize(new Player {
        //         id = Guid.NewGuid().ToString(),
        //         color = PlayerColor.Red,
        //     });
        //     uiController.AddPlayer(players[i].player);
        // }
        // currentPlayer = localPlayer;
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


    public void ChangeState(GameState newState)
    {
        if(state == GameState.PlayersCreateHouses && newState == GameState.Play)
        {
            mapController.EnableLocationBoxColliders(false);
        }
        state = newState;
    }

    public void EndTurn()
    {
        if(state == GameState.PlayersCreateHouses) {
            // TODO: Check if all players have created two houses and a worker
            if(currentPlayer.locations.Count == 2) {
                ChangeState(GameState.Play);
                currentPlayer.EnableTurn(true);
            }
        } else {
            currentPlayer.SetState(PlayerController.State.WaitForTurn);
            currentPlayer.EnableTurn(false);


        }
    }

    // This function is only here temporarly for testing
    public void GainResources() {
        (int wood, int stone, int clay, int wheat, int wool) = mapController.CalculateGainableResources(currentPlayer.player);

        currentPlayer.player.wood += wood;
        currentPlayer.player.stone += stone;
        currentPlayer.player.clay += clay;
        currentPlayer.player.wheat += wheat;
        currentPlayer.player.wool += wool;

        // Update UI
        uiController.UpdatePlayerUI(currentPlayer.player);
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
