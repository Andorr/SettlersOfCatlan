using State;
using UnityEngine;

public class WorkerHandler : MonoBehaviour, IActionHandler
{
    private WorkerController workerController;
    private LocationController[] currentAvailableMoves;
    private PathController[] adjecentPaths;

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

        // Show UI action panel
        controller.uiController.EnableActionPanel(
            true, 
            () => EnableRoadPlacement(controller),
            () => EnableHousePlacement(controller)
        );

        // Initialize moves to show
        if(workerController.state == WorkerController.WorkerState.Movable)
        {
            currentAvailableMoves = controller.mapController.GetAdjecentLocations(workerController.worker.location);
            foreach(var lc in currentAvailableMoves)
            {
                lc.SetSelectable(true);
            }
            shouldTryOverride = true;
            controller.localPlayer.SetState(PlayerController.State.WorkerMovement);
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
        if(controller.localPlayer.state == PlayerController.State.WorkerMovement)
        {
            var lc = other.GetComponent<LocationController>();
            if(lc == null) {
                return false;
            }
            controller.localPlayer.MoveWorker(workerController, lc.location);
            workerController.state = WorkerController.WorkerState.Immovable;
            shouldTryOverride = true;
            return true;
        }
        else if(controller.localPlayer.state == PlayerController.State.PathPlacement)
        {
            var pc = other.GetComponent<PathController>();
            if(pc == null) {
                return false;
            }
            
            controller.GetLocalPlayer().BuildPath(pc);
            return true;
        }
        return false;
    }

    private void EnableRoadPlacement(GameController controller) {
        // Enable all path colliders to adjecent paths
        adjecentPaths = controller.mapController.GetAdjecentPaths(workerController.worker.location);
        foreach(var path in adjecentPaths)
        {
            path.SetSelectable(true);
        }
        controller.GetLocalPlayer().SetState(PlayerController.State.PathPlacement);
        shouldTryOverride = true;
        DisableMovement();
        EnableWorker(false);
    }

    private void EnableHousePlacement(GameController controller) {

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
