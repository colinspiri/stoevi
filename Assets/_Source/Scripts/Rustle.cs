using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SpookuleleAudio;
using UnityEngine;

public class Rustle : MonoBehaviour {
    // constants
    public FloatReference loudnessIncrease;
    public FloatReference walkLoudness;
    public FloatReference runLoudness;
    
    // state
    private bool playerInside;

    private void Update() {
        if (playerInside) {
            bool moving = FirstPersonMovement.Instance.moveState == FirstPersonMovement.MoveState.Walking ||
                          FirstPersonMovement.Instance.moveState == FirstPersonMovement.MoveState.Running;

            if (moving) {
                // play rustle sound
                AudioManager.Instance.SetRustle(true);
            
                // report sound
                float loudness = loudnessIncrease + FirstPersonMovement.Instance.moveState switch {
                    FirstPersonMovement.MoveState.Walking => walkLoudness,
                    FirstPersonMovement.MoveState.Running => runLoudness,
                };
                if(TorbalanHearing.Instance) TorbalanHearing.Instance.ReportSound(FirstPersonMovement.Instance.transform.position, loudness);
            }
            else AudioManager.Instance.SetRustle(false);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == FirstPersonMovement.Instance.gameObject) {
            playerInside = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject == FirstPersonMovement.Instance.gameObject) {
            playerInside = false;
            AudioManager.Instance.SetRustle(false);
        }
    }

    private void OnDrawGizmosSelected() {
        // loudness radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loudnessIncrease);
    }
}
