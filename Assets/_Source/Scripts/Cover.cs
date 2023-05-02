using System;
using BehaviorDesigner.Runtime.Tasks.Unity.Math;
using UnityEngine;

public class Cover : MonoBehaviour
{
    // components
    public CoverSet coverSet;
    public BushSet bushSet;
    
    // public constants
    public enum CoverType { Sparse, Complete }
    public CoverType type;
    public bool isBush;

    // state
    public bool playerInside { get; private set; }
    public bool torbalanInside { get; private set; }

    private void Start() {
        playerInside = false;
    }

    private void OnEnable() {
        coverSet.Add(this);
        if(isBush && bushSet != null) bushSet.Add(this);
    }
    private void OnDisable() {
        coverSet.Remove(this);
        if(isBush && bushSet != null) bushSet.Remove(this);
    }

    private void Update() {
        if (playerInside && isBush) {
            if(ObjectiveUI.Instance != null) ObjectiveUI.Instance.FinishPrompt("Bush");
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (FirstPersonMovement.Instance != null && other.gameObject == FirstPersonMovement.Instance.gameObject) {
            playerInside = true;
        }
        if (TorbalanDirector.Instance != null && other.gameObject == TorbalanDirector.Instance.gameObject) {
            torbalanInside = true;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (FirstPersonMovement.Instance != null && other.gameObject == FirstPersonMovement.Instance.gameObject) {
            playerInside = false;
        }
        if (TorbalanDirector.Instance != null && other.gameObject == TorbalanDirector.Instance.gameObject) {
            torbalanInside = false;
        }
    }
}
