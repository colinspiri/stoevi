using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefillSeeds : Interactable
{
    public override bool IsInteractable() {
        return false;
    }

    public override void Interact() {
        
    }

    public override string GetObjectName() {
        return "seed crate";
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPrompt() {
        return "temporarily disabled";
    }
}
