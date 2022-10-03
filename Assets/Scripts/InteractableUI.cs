using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableUI : MonoBehaviour {
    // components
    public TextMeshProUGUI interactableText;

    public void ClearInteractableUI() {
        interactableText.text = "";
    }

    public void ShowSelectedObject(Interactable selectedObject) {
        interactableText.text = selectedObject.GetUIText();
    }
}
