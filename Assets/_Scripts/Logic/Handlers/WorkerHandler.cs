using UnityEngine;

public class WorkerHandler : MonoBehaviour, IActionHandler
{
    public void OnHover(GameController controller)
    {
        Debug.Log("Hello world! :D");
    }

    public void OnSelected(GameController controller)
    {
        Debug.Log("I am clicked!");
    }

    public void OnUnselected(GameController controller)
    {
    }
}
