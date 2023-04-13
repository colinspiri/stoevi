using SpookuleleAudio;
using UnityEngine;
using UnityEngine.Serialization;

public class Sheep : Interactable {
    // components
    public ASoundContainer sheep_hit;
    
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
        if(SheepManager.Instance != null) SheepManager.Instance.AddSheep(this);
    }

    private void Update() {
        player = (FirstPersonMovement.Instance != null) ? FirstPersonMovement.Instance.gameObject : null;

        beingChased = CheckIfBeingChased();
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
        return "sheep";
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPromptPrimary() {
        return GetInteractPrimaryButton() + " hit sheep";
    }

    protected void OnDestroy() {
        if(SheepManager.Instance != null) SheepManager.Instance.RemoveSheep(this);
    }
}
