using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeftRightClickable : MonoBehaviour, IPointerClickHandler
{
    private bool interactable = true;
    public UnityEvent onLeft;
    public UnityEvent onRight;

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!interactable) {
            return;
        }

        if(eventData.button == PointerEventData.InputButton.Left) {
            onLeft.Invoke();
        } else if(eventData.button == PointerEventData.InputButton.Right) {
            onRight.Invoke();
        }
    }

    public void Interactable(bool enable) {
        interactable = enable;

        Image graphic = GetComponent<Image>();
        if(graphic == null) {
            return;
        }

        if(!interactable) {
            graphic.color = new Color(157f/255f, 157f/255f, 157f/255f, 1f);
        } else {
            graphic.color = Color.white;
        }
    }
}
