using UnityEngine;

public class WorkerHandler : MonoBehaviour, IActionHandler
{
    public void OnHover(GameController controller)
    {
        Debug.Log("Hello world! :D");
    }

    public OnSelected(GameController controller)
    {
        Debug.Log("I am clicked!");
        Debug.Log(this.GetComponent<WorkerController>().worker);
    }

    public void OnUnselected(GameController controller)
    {
    }
}
