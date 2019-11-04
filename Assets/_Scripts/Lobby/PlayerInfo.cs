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
    private string _defaultName = "Default Name";
    public PlayerInfo(InputField inputField){
        this._inputField = inputField;
    }

    // return true if the name is valid and return false if its not
    public string SetPlayerName(){
        string name = this._inputField.text;

        if(!string.IsNullOrEmpty(name)){
            name = name.Trim();
            return name;
        }
        return _defaultName;
    }
}
