using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using SpookuleleAudio;
using UnityEngine;
using UnityEngine.AI;

public class SheepAnimatorController : MonoBehaviour {
    // components
    public Animator animator;
    public NavMeshAgent agent;
    public ASoundContainer bleat;
    
    // constants
    public float walkingSpeedThreshold;
    public float runningSpeedThreshold;

    // Update is called once per frame
    void Update() {
        float currentSpeed = agent.velocity.magnitude;
        bool walking = currentSpeed > walkingSpeedThreshold;
        bool running = currentSpeed > runningSpeedThreshold;
        
        // set animator params
        animator.SetBool("walking", walking);
        animator.SetBool("running", running);
    }

    public void PlayEatingAnimation() {
        animator.SetTrigger("startEating");
    }
    public void StopEatingAnimation() {
        animator.SetTrigger("stopEating");
    }

    public void Bleat() {
        bleat.Play3D(transform);

        if (animator.GetBool("walking") == false && animator.GetBool("running") == false) {
            animator.SetTrigger("bleat");
        }
    }
}