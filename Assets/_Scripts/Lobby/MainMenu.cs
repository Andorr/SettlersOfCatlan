using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
public class MainMenu
{    
    private Button newGame;
    private Button joinGame;
    private Button exitGame;

    // the main menu contains of New game, Join Game,
    public MainMenu(Button newGameButton, Button joinGameButton, Button exitGameButton){
        this.newGame = newGameButton;
        this.joinGame = joinGame;
        this.exitGame = exitGameButton;
    }

    public void NewGame(){

    }

    public void JoinGame(){

    }

    public void ExitGame(){
        Application.Quit();
    }
}
