using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private PlayerController currentPlayer;

    public void Start() {
        currentPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public PlayerController GetCurrentPlayer() {
        return currentPlayer;
    }
}
