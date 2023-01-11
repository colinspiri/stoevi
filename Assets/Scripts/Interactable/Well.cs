using System;
using System.Collections;
using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;

public class Well : Interactable {
    // components
    public ASoundContainer well;
    
    // shared state
    public IntVariable maxWater;
    public IntVariable currentWater;
    
    public override void Interact() {
        currentWater.SetValue(maxWater);
    }

    public override void OnStartInteracting() {
        base.OnStartInteracting();
        well.Play3D(transform);
    }

    public override string GetObjectName() {
        return "well";
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPrompt() {
        return (currentWater.Value == maxWater.Value) ? "water is already full" : "E to refill water";
    }

    public override bool IsInteractable() {
        return currentWater.Value < maxWater.Value;
    }
}
