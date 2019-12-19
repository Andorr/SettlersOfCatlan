using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public class WorkerController : MonoBehaviour
{
    [SerializeField]
    public Worker worker;
    public Transform prefabHolder;
    private Animator anim;

    public enum WorkerState {
        Movable,
        Immovable,
    }
    public WorkerState state = WorkerState.Movable;

    public void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void Initialize(Worker newWorker)
    {
        worker = newWorker;
    }

    public void MoveWorker(Location location)
    {
        worker.location = location;
        StartCoroutine(MoveWorkerWithAnim(location));
        state = WorkerState.Immovable;
    }

    public void EnableWorker(bool enable) {
        GetComponent<BoxCollider>().enabled = enable;

        if(enable) {
            state = WorkerState.Movable;
        } else {
            state = WorkerState.Immovable;
        }
    }

    private IEnumerator MoveWorkerWithAnim(Location location)
    {
        
        anim.SetTrigger("Shrink");
        yield return new WaitForSeconds(1);
        transform.position = location.position;
        anim.SetTrigger("Grow");
    }
}
