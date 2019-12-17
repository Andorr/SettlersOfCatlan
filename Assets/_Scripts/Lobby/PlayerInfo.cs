using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerInfo
{    
    private InputField _inputField;
    private Dictionary<string, Player> players;
    const string playerNamePrefKey = "PlayerName";
    const string _defaultName = "Default Name";

    // return true if the name is valid and return false if its not
    public void SetPlayerName(string name){
       //Checking if the name is null or empty, if it is then the user will recive a default name 
	    if(!string.IsNullOrEmpty(name)){
            name = name.Trim();
            PlayerPrefs.SetString(playerNamePrefKey,name);
        }else{
            PlayerPrefs.SetString(playerNamePrefKey, _defaultName);
        }
    }



    public void SetPlayers(){

    }

    public Dictionary<string, Player> GetPlayers(){
        return players;
    }

}
