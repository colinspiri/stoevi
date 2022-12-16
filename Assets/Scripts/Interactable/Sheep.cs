using System;
using System.Collections;
using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;

public class Sheep : Interactable {
    // components
    public ASoundContainer bleat;
    
    // behavior tree shared variables
    public bool scared { get; set; }
    public GameObject player { get; set; }
    
    // public constants
    public float bleatLoudness;

    private void Update() {
        player = FirstPersonController.Instance.gameObject;

        // if player is nearby & running towards sheep, become scared
    }

    public override void Interact() {
        scared = true;
        bleat.Play3D(transform);

        // report sound to torbalan
        if (TorbalanSenses.Instance) {
            TorbalanSenses.Instance.ReportSound(transform.position, bleatLoudness);
        }
    }

    public void CalmDown() {
        scared = false;
    }

    public override string GetObjectName() {
        return "sheep";
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPrompt() {
        return "E to hit sheep";
    }
}
