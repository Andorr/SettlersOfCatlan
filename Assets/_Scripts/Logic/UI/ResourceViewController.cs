using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;
using UnityEngine.UI;

public class ResourceViewController : MonoBehaviour
{
    [Header("ResourceTexts")]
    public Text WoodText;
    public Text StoneText;
    public Text ClayText;
    public Text WheatText;
    public Text WoolText;

    public void UpdateResourceCount(Player player) {
        var resourceStore = player.resources;
        WoodText.text = resourceStore.wood.ToString();
        StoneText.text = resourceStore.stone.ToString();
        ClayText.text = resourceStore.clay.ToString();
        WheatText.text = resourceStore.wheat.ToString();
        WoolText.text = resourceStore.wool.ToString();
    }
}
