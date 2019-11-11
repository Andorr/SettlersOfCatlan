using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace State
{
    public enum PlayerColor
    {
        Red,
        Blue,
        Green,
        Purple,
    }


    [System.Serializable]
    public class Player
    {
        public string id;
        public PlayerColor color;

        public Color GetColor() {
            switch(color) {
                case PlayerColor.Red: {
                    return new Color(245/255f, 66/255f, 66/255f);
                }
                case PlayerColor.Blue: {
                    return new Color(66/255f, 135/255f, 245/255f);
                }
                case PlayerColor.Green: {
                    return new Color(66/255f, 245/255f, 117/255f);
                }
                case PlayerColor.Purple: {
                    return new Color(188/255f, 66/255f, 245/255f);
                }
                default: {
                    return Color.black;
                }
            }
        }
    }
    
}
