using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Well : Interactable {
    public override void Interact() {
        base.Interact();
        
        WaterUI.Instance.RefillWater();
    }

    public override string GetUIText() {
        return WaterUI.Instance.IsWaterFull() ? "water is already full" : "E to refill water";
    }
}
