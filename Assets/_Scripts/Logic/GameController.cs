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

    public PlayerController localPlayer { get; set; }
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

        mapController.GenerateMap();
        mapController.EnableLocationBoxColliders(true);

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
            if(localPlayer.locations.Count == 2) {
                ChangeState(GameState.Play);
                localPlayer.EnableTurn(true);
            }
        } else {
            localPlayer.SetState(PlayerController.State.WaitForTurn);
            localPlayer.EnableTurn(false);


        }
    }

    // This function is only here temporarly for testing
    // THIS METHOD HAS ALREADY BEEN REPLACED IN PlayerController, SO REMOVE THIS WHEN THE "GAIN RESOURCES" BUTTON IS NO LONGER NEEDED!
    public void GainResources() {
        (int wood, int stone, int clay, int wheat, int wool) = mapController.CalculateGainableResources(localPlayer.player);
        localPlayer.AddResources(wood, stone, clay, wheat, wool);
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
