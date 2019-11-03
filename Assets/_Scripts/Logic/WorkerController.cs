using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public class WorkerController : MonoBehaviour
{
    [SerializeField]
    public Worker worker;
    public Transform prefabHolder;

    private bool isSelectable = false;

    public void Initialize(Worker newWorker)
    {
        worker = newWorker;
    }
}
