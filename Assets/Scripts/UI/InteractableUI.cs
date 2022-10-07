using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableUI : MonoBehaviour {
    // components
    public TextMeshProUGUI interactableText;

    private void Start() {
        if (InteractableManager.Instance != null) {
            InteractableManager.Instance.onSelectedObjectChange.AddListener(ShowSelectedObject);
        }
    }

    private void ShowSelectedObject(Interactable selectedObject) {
        if (selectedObject == null) interactableText.text = "";
        else interactableText.text = selectedObject.GetUIText();
    }
}
