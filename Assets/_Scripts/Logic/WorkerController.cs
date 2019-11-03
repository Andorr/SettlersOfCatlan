using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public class WorkerController : MonoBehaviour
{
    [SerializeField]
    public Worker worker;
    public Transform prefabHolder;

    public enum WorkerState {
        Movable,
        Immovable,
    }
    public WorkerState state = WorkerState.Movable;

    private bool isSelectable = false;

    public void Initialize(Worker newWorker)
    {
        worker = newWorker;
    }
}
