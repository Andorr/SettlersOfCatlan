using UnityEngine;
using UnityEngine.UI;

public class ResourceItemController : MonoBehaviour
{
    public Text text;

    [Header("Resource Items")]
    public GameObject wood;
    public GameObject stone;
    public GameObject clay;
    public GameObject wheat;
    public GameObject wool;

    public void ShowResources(string playerName, ResourceStorage storage) {
        Debug.Log("ShowResources called!");

        gameObject.SetActive(true);

        text.text = $"{playerName} gained:";
        text.GetComponent<GraphicFade>().FadeInAndOut(1f, 5f);
        if (storage.IsEmpty()) {
            text.text = $"{playerName} gained nothing";
            wood.SetActive(false);
            stone.SetActive(false);
            clay.SetActive(false);
            wheat.SetActive(false);
            wool.SetActive(false);
            return;
        }
        

        ShowResource(storage.wood, this.wood);
        ShowResource(storage.stone, this.stone);
        ShowResource(storage.clay, this.clay);
        ShowResource(storage.wheat, this.wheat);
        ShowResource(storage.wool, this.wool);
    }

    private void ShowResource(int value, GameObject obj) {
        obj.SetActive(value > 0);

        if(value > 0) {
            obj.GetComponentInChildren<Text>().text = $"{value}x";
            Fade(obj);
        }
    }

    private void Fade(GameObject obj) {
        foreach(var fader in obj.GetComponentsInChildren<GraphicFade>()) {
            fader.FadeInAndOut(1f, 5f);
        }
    }
}
