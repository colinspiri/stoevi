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

    public override string GetUIText() {
        if (ResourceManager.Instance.carryingFertilizer) {
            return "already carrying fertilizer";
        }
        return "E to pick up fertilizer";
    }
}
