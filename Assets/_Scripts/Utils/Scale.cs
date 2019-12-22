using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scale : MonoBehaviour
{
    public Transform rect;

    public Vector3 maxScale = new Vector3(1.1f, 1.1f, 1.1f);
    public Vector3 minScale = new Vector3(1f, 1f, 1f);

    public void Start() {
        if(rect == null) {
            rect = GetComponent<RectTransform>();
        }
    }

    public void ScaleUp() {
        rect.localScale = maxScale;
    }

    public void ScaleDown() {
        rect.localScale = minScale;
    }
}
