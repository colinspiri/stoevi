using SpookuleleAudio;
using UnityEngine;
using UnityEngine.Serialization;

public class Sheep : Interactable {
    // components
    public ASoundContainer sheep_hit;
    public IntVariable objectiveComplete;
    public TimeOfDay timeOfDay;
    
    // behavior tree shared variables
    public GameObject player { get; set; }
    public bool scared { get; set; }
    public bool beingChased { get; set; }
    public bool isLeashed { get; set; }
    
    // public constants
    public float bleatLoudness;
    [Header("Player Chase")]
    [FormerlySerializedAs("playerRunTowardsDistance")] public float playerChaseDistance;
    public float playerFacingAngle;

    [Header("Yoan-Sheep")] 
    public bool isYoan;
    public bool canBeLeashed;
    
    // state
    private float sneakTimer;

    protected override void Start() {
        base.Start();
        if(SheepManager.Instance != null) SheepManager.Instance.AddSheep(this);
    }

    private void Update() {
        player = (FirstPersonMovement.Instance != null) ? FirstPersonMovement.Instance.gameObject : null;

        beingChased = CheckIfBeingChased();

        if (isYoan && timeOfDay.IsNight() == false) {
            var distance = Vector3.Distance(player.transform.position, transform.position);
            if (distance <= playerChaseDistance * 2) {
                beingChased = true;
            }
        }
    }

    private bool CheckIfBeingChased() {
        // if player is nearby & running towards sheep, become scared
        if (player == null) return false;
        
        // player within radius
        var distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > playerChaseDistance) return false;

        // player running
        if (FirstPersonMovement.Instance.moveState != FirstPersonMovement.MoveState.Running) return false;
        
        // player facing sheep
        Vector3 playerToSheep = transform.position - player.transform.position;
        var angle = Vector3.Angle(player.transform.forward, playerToSheep);

        return angle < playerFacingAngle;
    }

    public override void InteractPrimary() {
        sheep_hit.Play3D(transform);
        CameraShake.Instance.Shake();
        
        BecomeScared();
    }

    public override bool IsInteractableSecondary() {
        return canBeLeashed;
    }

    public override void InteractSecondary() {
        if (canBeLeashed) {
            isLeashed = !isLeashed;
            if (isYoan) {
                objectiveComplete.Value = isLeashed ? 1 : 0;
            } 
        }
    }

    public void BecomeScared() {
        scared = true;
        
        // report sound to torbalan
        if (TorbalanHearing.Instance) {
            TorbalanHearing.Instance.ReportSound(transform.position, bleatLoudness);
        }
    }
    
    public void CalmDown() {
        scared = false;
    }

    public override string GetObjectName() {
        if (isYoan) return "Yoan";
        else return "sheep";
    }

    public override string GetObjectDescription() {
        if (canBeLeashed && isLeashed) return "leashed";
        return "";
    }

    public override string GetButtonPromptPrimary() {
        return GetInteractPrimaryButton() + " hit sheep";
    }

    public override string GetButtonPromptSecondary() {
        if (canBeLeashed) {
            return GetInteractSecondaryButton() + " " + (isLeashed ? "detach leash" : "attach leash");
        }

        return "";
    }

    protected void OnDestroy() {
        if(SheepManager.Instance != null) SheepManager.Instance.RemoveSheep(this);
    }
}
