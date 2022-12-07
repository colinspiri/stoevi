using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Well : Interactable {
    
    public override void Interact() {
        GameManager.Instance.RefillWater();
    }

    public override string GetUIText() {
        return GameManager.Instance.IsWaterFull() ? "water is already full" : "E to refill water";
    }
}
