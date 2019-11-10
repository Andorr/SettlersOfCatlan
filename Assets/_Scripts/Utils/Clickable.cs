using UnityEngine;
using UnityEngine.EventSystems;

public class Clickable : MonoBehaviour, IPointerClickHandler
{
    public delegate void ClickHandler();
    public event ClickHandler OnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(OnClick != null) 
        {
            OnClick();
        }
    }
}