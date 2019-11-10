using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public MapController mapController;
    public UIController uiController;
    public IActionHandler handler;

    public PlayerController localPlayer { get; private set; }
    private PlayerController currentPlayer;
    public enum GameState {
        PlayersCreateWorker,
        Play,
        End,
    }

    public GameState state = GameState.PlayersCreateWorker;

    private PlayerController[] players;

    # region Unity Methods
    public void Start() {
        mapController = GetComponent<MapController>();
        uiController = GetComponent<UIController>();

        mapController.GenerateMap();
        mapController.EnableLocationBoxColliders(true);

        // TODO: When on multiplayer, initiate player objects and set this to PhotonView.isMine
        localPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        players = new PlayerController[1] {
            localPlayer,
        };
        currentPlayer = localPlayer;
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
        if(state == GameState.PlayersCreateWorker && newState == GameState.Play)
        {
            mapController.EnableLocationBoxColliders(false);
        }
        state = newState;
    }

    public void EndTurn()
    {
        if(state == GameState.PlayersCreateWorker) {
            // TODO: Check if all players have created a worker
            ChangeState(GameState.Play);
        } else {
            currentPlayer.SetState(PlayerController.State.WaitForTurn);
            currentPlayer.EnableWorkers(false);


        }
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
