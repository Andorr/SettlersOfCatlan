using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public class PathController : MonoBehaviour
{

    private Path path;

    [Header("GameObjects")]
    public GameObject pathPrefab;
    private Transform pathHolder;
    public GameObject selectableIndicator;

    private bool isSelectable = false;

    public void Initialize(Path newPath, float radius)
    {
        path = newPath;
    }

    public void SetSelectable(bool selectable) {
        isSelectable = selectable;
        selectableIndicator.SetActive(selectable);
    }
}
