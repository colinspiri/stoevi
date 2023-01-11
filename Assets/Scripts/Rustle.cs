using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rustle : MonoBehaviour {
    public float loudness;

    private void OnTriggerStay(Collider other) {
        if (other.gameObject == FirstPersonMovement.Instance.gameObject) {
            if (FirstPersonMovement.Instance.moveState != FirstPersonMovement.MoveState.Still) {
                // play audio
                
                // report sound
                TorbalanHearing.Instance.ReportSound(FirstPersonMovement.Instance.transform.position, loudness);
            }
        }
    }
    
    private void OnDrawGizmosSelected() {
        // loudness radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loudness);
    }
}
