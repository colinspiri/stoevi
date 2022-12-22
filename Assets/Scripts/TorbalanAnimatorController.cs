using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TorbalanAnimatorController : MonoBehaviour {
    // components
    public Animator animator;
    public NavMeshAgent agent;
    
    // constants
    public float walkingSpeedThreshold;

    // Update is called once per frame
    void Update() {
        float currentSpeed = agent.velocity.magnitude;
        bool walking = currentSpeed > walkingSpeedThreshold;
        // Debug.Log("walking " + walking + " with speed = " + currentSpeed);
        animator.SetBool("walking", walking);
    }
}
