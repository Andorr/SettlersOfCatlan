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
    const string playerNamePrefKey = "PlayerName";
    const string _defaultName = "Default Name";

    // return true if the name is valid and return false if its not
    public void SetPlayerName(string name){
        if(!string.IsNullOrEmpty(name)){
            name = name.Trim();
            PlayerPrefs.SetString(playerNamePrefKey,name);
        }
        PlayerPrefs.SetString(playerNamePrefKey, _defaultName);
    }
}
