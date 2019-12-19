using UnityEngine;


public class LocationHandler : MonoBehaviour, IActionHandler
{
    private LocationController locationController;

    private bool shouldTryOverride = false;
    public bool TryOverride => shouldTryOverride;

    public void Start() {
        locationController = GetComponent<LocationController>();
    }

    public void OnHover(GameController controller)
    {

    }

    public void OnSelected(GameController controller)
    {
        if(controller.state == GameController.GameState.PlayersCreateHouses && locationController.location.type == State.LocationType.Available)
        {
            var location = locationController.location;
            var wc = controller.GetLocalPlayer().CreateWorker(location);
            wc.EnableWorker(false);
            controller.GetLocalPlayer().BuildHouse(location);
            controller.GetLocalPlayer().EndTurn();
        }
        else if(controller.state == GameController.GameState.Play)
        {
            
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
