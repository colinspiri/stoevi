using System;
using System.Collections;
using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;

public class Rustle : MonoBehaviour {
    public ASoundContainer soundToPlay;
    
    private static float crouchWalkingSoundDelayMultiplier = 2f;
    private static float runningSoundDelayMultiplier = 0.4f;
    
    // constants
    public float baseSoundDelayTime;
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
                    soundDelayTimer = baseSoundDelayTime * FirstPersonMovement.Instance.moveState switch {
                        FirstPersonMovement.MoveState.CrouchWalking => crouchWalkingSoundDelayMultiplier,
                        FirstPersonMovement.MoveState.Walking => 1,
                        FirstPersonMovement.MoveState.Running => runningSoundDelayMultiplier,
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
                if(TorbalanHearing.Instance) TorbalanHearing.Instance.ReportSound(FirstPersonMovement.Instance.transform.position, loudness);
            }
        }
    }
    
    private void OnDrawGizmosSelected() {
        // loudness radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loudnessIncrease);
    }
}
