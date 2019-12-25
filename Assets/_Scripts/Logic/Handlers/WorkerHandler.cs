using System.Linq;
using State;
using UnityEngine;

public class WorkerHandler : MonoBehaviour, IActionHandler
{
    private WorkerController workerController;
    private LocationController[] currentAvailableMoves;
    private PathController[] adjecentPaths;
    private Player player;

    private bool shouldTryOverride = false;
    public bool TryOverride => shouldTryOverride;

    public void Start() {
        workerController = GetComponent<WorkerController>();
    }

    public void OnHover(GameController controller)
    {
        
    }

    public void OnSelected(GameController controller)
    {   
        Player player = controller.GetLocalPlayer().player;

        // Show UI action panel
        controller.uiController.EnableActionButtons(
            ResourceUtil.CanAffordPath(player.resources) && 
                controller.mapController.GetAdjecentPaths(workerController.worker.location, true).Length > 0, 
            ResourceUtil.CanAffordHouse(player.resources) && 
                workerController.worker.location.type == LocationType.Available &&
                controller.mapController.GetConnectedLocations(workerController.worker.location, player).Where(l => l.type != LocationType.Available).Count() > 0,
            ResourceUtil.CanAffordCity(player.resources) && 
                workerController.worker.location.type == LocationType.House && controller.GetLocalPlayer().player.id.Equals(workerController.worker.location.occupiedBy),
                ResourceUtil.CanAffordCard(player.resources));
        controller.uiController.EnableActionPanel(
            true, 
            () => EnableRoadPlacement(controller),
            () => BuildHouse(controller),
            () => BuildCity(controller)
        );

        // Initialize moves to show
        if(workerController.state == WorkerController.WorkerState.Movable)
        {
            currentAvailableMoves = controller.mapController.GetReachableLocations(workerController.worker.location, player);
            foreach(var lc in currentAvailableMoves)
            {
                lc.SetSelectable(true);
            }
            shouldTryOverride = true;
            controller.GetLocalPlayer().SetState(PlayerController.State.WorkerMovement);
        }
        
    }

    public void OnUnselected(GameController controller)
    {
        controller.uiController.EnableActionPanel(false);

        
        DisableMovement();
        DisablePathPlacement();
        EnableWorker(true);

        controller.GetLocalPlayer().SetState(PlayerController.State.None);
    }

    public bool OnTryOverride(GameController controller, GameObject other)
    {
        // Worker is selected, try to move the player
        var localPlayer = controller.GetLocalPlayer();
        if(localPlayer.state == PlayerController.State.WorkerMovement)
        {
            var lc = other.GetComponent<LocationController>();
            if(lc == null) {
                return false;
            }
            localPlayer.MoveWorker(workerController.worker, lc.location);
            shouldTryOverride = true;
            return true;
        }
        else if(localPlayer.state == PlayerController.State.PathPlacement)
        {
            var pc = other.GetComponent<PathController>();
            if(pc == null) {
                return false;
            }
            
            localPlayer.BuildPath(pc.path);
            return true;
        }
        return false;
    }

    private void EnableRoadPlacement(GameController controller) {
        // Enable all path colliders to adjecent paths
        adjecentPaths = controller.mapController.GetAdjecentPaths(workerController.worker.location, true);
        foreach(var path in adjecentPaths)
        {
            path.SetSelectable(true);
        }
        controller.GetLocalPlayer().SetState(PlayerController.State.PathPlacement);
        shouldTryOverride = true;
        DisableMovement();
        EnableWorker(false);
    }

    private void BuildHouse(GameController controller)
    {
        var exists = controller.mapController.GetLocationController(workerController.worker.location, out LocationController lc);
        if(!exists)
        {
            return;
        }
        controller.GetLocalPlayer().BuildHouse(lc.location);
        OnUnselected(controller);
    }

    private void BuildCity(GameController controller) 
    {
        var exists = controller.mapController.GetLocationController(workerController.worker.location, out LocationController lc);
        if(!exists)
        {
            return;
        }
        controller.GetLocalPlayer().BuildCity(lc.location);
        OnUnselected(controller);
    }

    private void DisableMovement()
    {
        if(currentAvailableMoves != null)
        {
            foreach(var lc in currentAvailableMoves)
            {
                lc.SetSelectable(false);
            }
            currentAvailableMoves = null;
        }
    }

    private void DisablePathPlacement()
    {
        if(adjecentPaths != null)
        {
            foreach(var pc in adjecentPaths)
            {
                pc.SetSelectable(false);
            }
            adjecentPaths = null;
        }
    }

    private void EnableWorker(bool enable)
    {
        GetComponent<BoxCollider>().enabled = enable;
    }
}
