using UnityEngine;

public interface IActionHandler {
    
    bool TryOverride { get; }
    void OnSelected(GameController controller);
    void OnUnselected(GameController controller);
    void OnHover(GameController controller);
    bool OnTryOverride(GameController controller, GameObject other);
}