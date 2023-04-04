using UnityEngine;
using UnityEngine.AI;

public class TorbalanAnimatorController : MonoBehaviour {
    // components
    public Animator animator;
    public NavMeshAgent agent;
    public AudioSource torbalan_walking;
    public AudioSource torbalan_running;
    
    // constants
    public float walkingSpeedThreshold;
    public float runningSpeedThreshold;

    // Update is called once per frame
    void Update() {
        float currentSpeed = agent.velocity.magnitude;
        bool running = currentSpeed > runningSpeedThreshold;
        bool walking = currentSpeed > walkingSpeedThreshold;
        
        // set animator params
        animator.SetBool("running", running);
        animator.SetBool("walking", walking);
        
        // set sfx
        if (running) {
            if(!torbalan_running.isPlaying) torbalan_running.Play();
            if(torbalan_walking.isPlaying) torbalan_walking.Stop();
        }
        else if (walking) {
            if(!torbalan_walking.isPlaying) torbalan_walking.Play();
            if(torbalan_running.isPlaying) torbalan_running.Stop();
        }
        else {
            if(torbalan_walking.isPlaying) torbalan_walking.Stop();
            if(torbalan_running.isPlaying) torbalan_running.Stop();
        }
    }
}
