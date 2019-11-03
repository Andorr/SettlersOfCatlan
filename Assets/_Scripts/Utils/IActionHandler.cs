using UnityEngine;

public interface IActionHandler {
    void OnSelected(GameController controller);
    void OnUnselected(GameController controller);
}