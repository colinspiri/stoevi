using System;
using System.Collections;
using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;

public class Rustle : MonoBehaviour {
    public ASoundContainer soundToPlay;
    
    private static float crouchWalkingSoundDelayTime = 0.5f;
    private static float walkingSoundDelayTime = 0.25f;
    private static float runningSoundDelayTime = 0.1f;
    
    // constants
    public FloatReference loudnessIncrease;
    public FloatReference breatheLoudness;
    public FloatReference walkLoudness;
    public FloatReference runLoudness;
    
    // state
    private float soundDelayTimer;

    private void Update() {
        if (soundDelayTimer >= 0) {
            soundDelayTimer -= Time.deltaTime;
            if (soundDelayTimer < 0) soundDelayTimer = 0;
        }
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject == FirstPersonMovement.Instance.gameObject) {
            if (FirstPersonMovement.Instance.moveState != FirstPersonMovement.MoveState.Still) {
                // play audio
                if (soundDelayTimer <= 0) {
                    soundToPlay.Play();
                    soundDelayTimer = FirstPersonMovement.Instance.moveState switch {
                        FirstPersonMovement.MoveState.CrouchWalking => crouchWalkingSoundDelayTime,
                        FirstPersonMovement.MoveState.Walking => walkingSoundDelayTime,
                        FirstPersonMovement.MoveState.Running => runningSoundDelayTime,
                        _ => 0
                    };
                }
                
                // report sound
                float loudness = loudnessIncrease + FirstPersonMovement.Instance.moveState switch {
                    FirstPersonMovement.MoveState.CrouchWalking => breatheLoudness,
                    FirstPersonMovement.MoveState.Walking => walkLoudness,
                    FirstPersonMovement.MoveState.Running => runLoudness,
                };
                Debug.Log(gameObject.name + " reported sound w loudness = " + loudness);
                TorbalanHearing.Instance.ReportSound(FirstPersonMovement.Instance.transform.position, loudness);
            }
        }
    }
    
    private void OnDrawGizmosSelected() {
        // loudness radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loudnessIncrease);
    }
}
