using System;
using System.Collections;
using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;
using UnityEngine.Serialization;

public class Sheep : Interactable {
    // components
    public ASoundContainer bleat;
    
    // behavior tree shared variables
    public GameObject player { get; set; }
    public bool scared { get; set; }
    public bool beingChased { get; set; }

    // public constants
    public float bleatLoudness;
    [Header("Player Chase")]
    [FormerlySerializedAs("playerRunTowardsDistance")] public float playerChaseDistance;
    public float playerFacingAngle;
    
    // state
    private float sneakTimer;

    protected override void Start() {
        base.Start();
        SheepManager.Instance.AddSheep(this);
    }

    private void Update() {
        player = FirstPersonController.Instance.gameObject;

        beingChased = CheckIfBeingChased();
    }

    private bool CheckIfBeingChased() {
        // if player is nearby & running towards sheep, become scared

        // player within radius
        var distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > playerChaseDistance) return false;

        // player running
        if (FirstPersonController.Instance.moveState != FirstPersonController.MoveState.Running) return false;
        
        // player facing sheep
        Vector3 playerToSheep = transform.position - player.transform.position;
        var angle = Vector3.Angle(player.transform.forward, playerToSheep);

        return angle < playerFacingAngle;
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

    protected override void OnDestroy() {
        base.OnDestroy();
        SheepManager.Instance.RemoveSheep(this);
    }
}