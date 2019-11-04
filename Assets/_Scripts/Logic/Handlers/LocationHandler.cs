using UnityEngine;


public class LocationHandler : MonoBehaviour, IActionHandler
{
    private bool shouldTryOverride = false;
    public bool TryOverride => shouldTryOverride;

    public void OnHover(GameController controller)
    {

    }

    public void OnSelected(GameController controller)
    {
        if(controller.state == GameController.GameState.PlayersCreateWorker)
        {
            var location = this.GetComponent<LocationController>().location;
            controller.GetLocalPlayer().CreateWorker(location);
            controller.EndTurn();
        }
        else if(controller.state == GameController.GameState.Play)
        {
            Debug.Log("Times change!");
        }
    }

    public bool OnTryOverride(GameController controller, GameObject other)
    {
        return false;
    }

    public void OnUnselected(GameController controller)
    {
    }
}
