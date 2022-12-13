using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefillSeeds : Interactable
{
    public override bool IsInteractable() {
        return ResourceManager.Instance.SeedsFull() == false;
    }

    public override void Interact() {
        ResourceManager.Instance.RefillSeeds();
    }

    public override string GetObjectName() {
        return "seed crate";
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPrompt() {
        return ResourceManager.Instance.SeedsFull() ? "seeds already full" : "E to refill seeds";
    }

    public override string GetUIText() {
        string uiText = "";

        if (ResourceManager.Instance.SeedsFull()) uiText += "seeds already full";
        else uiText += "E to refill seeds";
        
        return uiText;
    }
}
