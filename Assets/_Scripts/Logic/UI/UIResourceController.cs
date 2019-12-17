using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using UnityEngine.UI;

public class UIResourceController : MonoBehaviour
{
    [Header("ResourceTexts")]
    public Text WoodText;
    public Text StoneText;
    public Text ClayText;
    public Text WheatText;
    public Text WoolText;

    public void UpdateResourceCount(Player player) {
        WoodText.text = player.wood.ToString();
        StoneText.text = player.stone.ToString();
        ClayText.text = player.clay.ToString();
        WheatText.text = player.wheat.ToString();
        WoolText.text = player.wool.ToString();
    }
}
