using System;
using System.Collections;
using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;

public class Sheep : Interactable {
    // components
    public ASoundContainer bleat;
    
    // behavior tree shared variables
    public GameObject player { get; set; }
    public bool scared { get; set; }
    public bool beingChased { get; set; }

    // public constants
    public float bleatLoudness;
    public float playerRunTowardsDistance;
    public float playerFacingAngle;

    private void Update() {
        player = FirstPersonController.Instance.gameObject;

        CheckIfBeingChased();
    }

    private void CheckIfBeingChased() {
        // if player is nearby & running towards sheep, become scared

        // player within radius
        var distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > playerRunTowardsDistance) return;

        // player running
        if (FirstPersonController.Instance.GetMoveState != FirstPersonController.MoveState.Running) return;
        
        // player facing sheep
        Vector3 playerToSheep = transform.position - player.transform.position;
        var angle = Vector3.Angle(player.transform.forward, playerToSheep);

        if (angle < playerFacingAngle) {
            beingChased = true;
        }
        else beingChased = false;
    }

    public override void Interact() {
        // TODO: play hit sound
        BecomeScared();
    }

    public void BecomeScared() {
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
