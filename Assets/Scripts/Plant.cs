using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour {
    public float loudness;

    private void Rustle() {
        // play audio TODO
        
        // report sound
        TorbalanSenses.Instance.ReportSound(FirstPersonController.Instance.transform.position, loudness);
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject == FirstPersonController.Instance.gameObject) {
            if (FirstPersonController.Instance.moveState != FirstPersonController.MoveState.Still) {
                Rustle();
            }
        }
    }
    
    private void OnDrawGizmosSelected() {
        // loudness radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loudness);
    }
}
