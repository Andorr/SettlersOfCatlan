using UnityEngine;

public class WorkerHandler : MonoBehaviour, IActionHandler
{
    private WorkerController workerController;
    private LocationController[] currentAvailableMoves;

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
        controller.uiController.EnableActionPanel(true);

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

        if(currentAvailableMoves != null)
        {
            foreach(var lc in currentAvailableMoves)
            {
                lc.SetSelectable(false);
            }
            currentAvailableMoves = null;
            shouldTryOverride = false;
        }
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
            return true;
        }
        return false;
    }
}
