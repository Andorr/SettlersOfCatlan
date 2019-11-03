using UnityEngine;

public class WorkerHandler : MonoBehaviour, IActionHandler
{
    private WorkerController workerController;
    private LocationController[] currentAvailableMoves;

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
                
            }
        }
        
    }

    public void OnUnselected(GameController controller)
    {
        controller.uiController.EnableActionPanel(false);
    }
}
