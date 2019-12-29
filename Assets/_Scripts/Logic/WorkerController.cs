using System.Collections;
using System.Collections.Generic;
using State;
using UnityEngine;

public class WorkerController : MonoBehaviour
{
    [SerializeField]
    public Worker worker;
    private Animator anim;

    private AudioController audioController;

    [Header("AudioClips")]
    public AudioClip[] soundEffects;

    public enum WorkerState {
        Movable,
        Immovable,
    }
    public WorkerState state = WorkerState.Movable;

    public void Start()
    {
        anim = GetComponentInChildren<Animator>();
        audioController = GameObject.FindGameObjectWithTag("AudioController").GetComponent<AudioController>();
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

    public void PlayRandomSound() {
        if(soundEffects == null || soundEffects.Length == 0) {
            return;
        }

        // Do not always play a sound
        if(Random.Range(0, 3) == 0) {
            return;
        }

        var audioClip = soundEffects[Random.Range(0, soundEffects.Length)];
        audioController.PlayClip(audioClip);
    }
}
