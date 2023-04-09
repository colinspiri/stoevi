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
    public AudioSource sheep_walking;
    public AudioSource sheep_running;
    
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
        
        // set sfx
        if (running) {
            if(!sheep_running.isPlaying) sheep_running.Play();
            if(sheep_walking.isPlaying) sheep_walking.Stop();
        }
        else if (walking) {
            if(!sheep_walking.isPlaying) sheep_walking.Play();
            if(sheep_running.isPlaying) sheep_running.Stop();
        }
        else {
            if(sheep_walking.isPlaying) sheep_walking.Stop();
            if(sheep_running.isPlaying) sheep_running.Stop();
        }
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