using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Well : Interactable {
    
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
        return ResourceManager.Instance.IsWaterFull() ? "water is already full" : "E to refill water";
    }

    public override bool IsInteractable() {
        return ResourceManager.Instance.IsWaterFull() == false;
    }
}
