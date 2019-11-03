using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public MapController mapController;
    public UIController uiController;
    public PlayerController localPlayer { get; private set; }
    public IActionHandler handler;

    public enum GameState {
        PlayersCreateWorker,
        Play,
        End,
    }
    public GameState state = GameState.PlayersCreateWorker;

    public void Start() {  
        mapController = GetComponent<MapController>();
        uiController = GetComponent<UIController>();

        mapController.GenerateMap();
        mapController.EnableLocationBoxColliders(true);
        localPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public PlayerController GetLocalPlayer() {
        return localPlayer;
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

        }
    }

    public void HandleMouseClick() {
        RaycastHit hit;
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(r, out hit)) {
            var actionHandler = hit.collider.gameObject.GetComponent<IActionHandler>();
            if(actionHandler == null) {
                return;
            }

            if(handler != null) {
                handler.OnUnselected(this);
            }
            handler = actionHandler;
            handler.OnSelected(this);
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
}
