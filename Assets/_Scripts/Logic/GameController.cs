using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerController currentPlayer { get; private set; }

    public void Start() {
        currentPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public PlayerController GetCurrentPlayer() {
        return currentPlayer;
    }
}
