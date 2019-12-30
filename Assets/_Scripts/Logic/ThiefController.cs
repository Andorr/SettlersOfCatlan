using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ThiefController : MonoBehaviour {

    public bool destroyOnFinishedAnimation = false;

    private Animator animator;
    public void Start() {
        animator = GetComponent<Animator>();
    }
    
    public void Update() {
        if (destroyOnFinishedAnimation && !animator.IsInTransition(0)) {
            Destroy(this.gameObject);
        }
    }
}
