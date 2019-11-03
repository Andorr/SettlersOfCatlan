using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerController currentPlayer { get; private set; }
    public IActionHandler handler;

    public void Start() {   
        currentPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public PlayerController GetCurrentPlayer() {
        return currentPlayer;
    }

    public void Update() {
        if(Input.GetMouseButtonDown(0)) {
            HandleMouseClick();
        }
        HandleMouseHover();
    }

    public void HandleMouseClick() {
        RaycastHit hit;
        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(r, out hit)) {
            Debug.Log("Hit: " + hit.collider.name);
            var actionHandler = hit.collider.gameObject.GetComponent<IActionHandler>();
            if(actionHandler == null) {
                return;
            }
   
            handler.OnUnselected(this);
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
}
