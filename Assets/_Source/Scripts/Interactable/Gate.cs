using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : Interactable {
    
    public override void Interact() {
        GameManager.Instance.GameOver(true);
    }

    public override string GetObjectName() {
        return "gate";
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPrompt() {
        return "hold " + GetInteractButton() + " to go home";
    }
}
