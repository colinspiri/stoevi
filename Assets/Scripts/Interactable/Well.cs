using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Well : Interactable {
    // shared state
    public IntVariable maxWater;
    public IntVariable currentWater;
    
    public override void Interact() {
        ResourceManager.Instance.RefillWater();
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
