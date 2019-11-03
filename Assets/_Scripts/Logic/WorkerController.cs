using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public class WorkerController : MonoBehaviour
{
    [SerializeField]
    public Worker worker;
    public Transform prefabHolder;
    public GameObject selectableIndicator;

    private bool isSelectable = false;

    public void Initialize(Worker newWorker, Location location, float radius)
    {
        worker = newWorker;

        transform.position = location.position;
        transform.GetComponent<BoxCollider>().size = new Vector3(radius/2, 1.5f, radius/2);
    }

    public void SetSelectable(bool selectable) {
        isSelectable = selectable;
        transform.GetComponent<BoxCollider>().enabled = selectable;
        selectableIndicator.SetActive(selectable);
    }
}
