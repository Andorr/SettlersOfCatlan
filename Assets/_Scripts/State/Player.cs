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
        public string name;
        public int victoryPoints;
        public PlayerColor color;

        public int wood = 0;
        public int stone = 0;
        public int clay = 0;
        public int wheat = 0;
        public int wool = 0;

        public Color GetColor() {
            switch(color) {
                case PlayerColor.Red: {
                    return new Color(245/255f, 66/255f, 66/255f);
                }
                case PlayerColor.Blue: {
                    return new Color(66/255f, 135/255f, 245/255f);
                }
                case PlayerColor.Green: {
                    return new Color(48/255f, 178/255f, 85/255f);
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
