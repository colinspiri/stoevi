using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fertilizer : Interactable {
    
    public override bool IsInteractable() {
        return !ResourceManager.Instance.carryingFertilizer;
    }

    public override void Interact() {
        ResourceManager.Instance.PickUpFertilizer();
        Destroy(gameObject);
    }

    public override string GetObjectName() {
        return "fertilizer";
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPrompt() {
        if (ResourceManager.Instance.carryingFertilizer) {
            return "already carrying fertilizer";
        }
        return GetInteractButton() + " to pick up fertilizer";    
    }
}
