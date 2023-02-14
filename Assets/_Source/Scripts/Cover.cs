using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    // components
    public CoverSet coverSet;
    
    // public constants
    public enum CoverType { Sparse, Complete }
    public CoverType type;

    // state
    [HideInInspector] public bool playerInside;

    private void Start() {
        playerInside = false;
    }

    private void OnEnable() {
        coverSet.Add(this);
    }
    private void OnDisable() {
        coverSet.Remove(this);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject == FirstPersonMovement.Instance.gameObject) {
            playerInside = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject == FirstPersonMovement.Instance.gameObject) {
            playerInside = false;
        }
    }
}
