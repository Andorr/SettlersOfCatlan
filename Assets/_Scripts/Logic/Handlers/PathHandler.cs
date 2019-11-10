using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHandler : MonoBehaviour, IActionHandler
{
    public bool TryOverride => false;

    public void OnHover(GameController controller)
    {
        
    }

    public void OnSelected(GameController controller)
    {
        
    }

    public bool OnTryOverride(GameController controller, GameObject other)
    {
        return true;
    }

    public void OnUnselected(GameController controller)
    {
        
    }
}
