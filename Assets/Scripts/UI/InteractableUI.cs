using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class InteractableUI : MonoBehaviour {
    // components
    public TextMeshProUGUI interactableText;
    public Slider interactableSlider;

    private void Start() {
        HideInteractableUI();
    }
    
    private void Update() {
        // TODO: refactor so that each interactable has UpdateUI() function to call when values change
        switch (InteractableManager.Instance.interactionState)
        {
            case InteractableManager.InteractionState.None:
                if (ResourceManager.Instance.carryingFertilizer) {
                    interactableText.gameObject.SetActive(true);
                    interactableText.text = "E to drop fertilizer";
                    interactableSlider.gameObject.SetActive(false);
                }
                else HideInteractableUI();
                break;
            case InteractableManager.InteractionState.Selecting:
                ShowSelectedObject();
                break;
            case InteractableManager.InteractionState.Interacting:
                ShowInteractingObject();
                break;
        }
    }

    private void HideInteractableUI() {
        interactableText.text = "";
        interactableText.gameObject.SetActive(false);
        interactableSlider.gameObject.SetActive(false);
    }

    private void ShowSelectedObject() {
        var selectedObject = InteractableManager.Instance.SelectedObject;
        
        interactableText.gameObject.SetActive(true);
        interactableText.text = selectedObject.GetUIText();
        
        if (selectedObject.GetSliderFloat() != 0) {
            interactableSlider.gameObject.SetActive(true);
            interactableSlider.value = selectedObject.GetSliderFloat();
        }
        else interactableSlider.gameObject.SetActive(false);
    }

    private void ShowInteractingObject() {
        interactableText.gameObject.SetActive(true);
        interactableText.text = InteractableManager.Instance.SelectedObject.GetUIText();
        
        interactableSlider.gameObject.SetActive(true);
        interactableSlider.value = InteractableManager.Instance.GetInteractingFloat();
    }
}
