using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public class PathController : MonoBehaviour
{

    public Path path;

    [Header("GameObjects")]
    public GameObject pathPrefab;
    public Transform pathHolder;
    public GameObject selectableIndicator;

    private bool isSelectable = false;

    public void Initialize(Path newPath, float radius)
    {
        path = newPath;

        transform.GetComponent<BoxCollider>().size = new Vector3(radius/2, 1.5f, radius/2);
    }

    public void SetSelectable(bool selectable) {
        isSelectable = selectable;
        transform.GetComponent<BoxCollider>().enabled = selectable;
        selectableIndicator.SetActive(selectable);
    }
}
