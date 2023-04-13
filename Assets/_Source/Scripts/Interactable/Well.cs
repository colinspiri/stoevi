using SpookuleleAudio;
using UnityEngine;

public class Well : Interactable {
    // components
    public AudioSource well;
    
    // shared state
    public IntVariable maxWater;
    public IntVariable currentWater;

    protected override void Start() {
        base.Start();
        this.InteractionTimePrimary = well.clip.length;
    }

    public override void InteractPrimary() {
        currentWater.SetValue(maxWater);
    }

    public override void OnStartInteractingPrimary() {
        base.OnStartInteractingPrimary();
        
        well.Play();
    }

    public override void OnStopInteractingPrimary() {
        base.OnStopInteractingPrimary();
        
        well.Stop();
    }

    public override string GetObjectName() {
        return "well";
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPromptPrimary() {
        return (currentWater.Value == maxWater.Value) ? "water is already full" : GetInteractPrimaryButton() + " refill water";
    }

    public override bool IsInteractablePrimary() {
        return currentWater.Value < maxWater.Value;
    }
}
