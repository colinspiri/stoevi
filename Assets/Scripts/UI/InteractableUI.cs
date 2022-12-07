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
        
        if (InteractableManager.Instance != null) {
            InteractableManager.Instance.onSelectedObjectChange.AddListener(newSelectedObject => {
                if (newSelectedObject == null) {
                    HideInteractableUI();
                }
                else ShowSelectedObject();
            });
        }
    }

    private void HideInteractableUI() {
        interactableText.text = "";
        interactableText.gameObject.SetActive(false);
        interactableSlider.gameObject.SetActive(false);
    }

    private void ShowSelectedObject() {
        var selectedObject = InteractableManager.Instance.GetSelectedObject();
        
        interactableText.gameObject.SetActive(true);
        interactableText.text = selectedObject.GetUIText();
        
        if (selectedObject.GetSliderFloat() != 0) {
            interactableSlider.gameObject.SetActive(true);
            interactableSlider.value = selectedObject.GetSliderFloat();
        }
        else interactableSlider.gameObject.SetActive(false);
    }

    private void Update() {
        if(InteractableManager.Instance.GetSelectedObject() != null) ShowSelectedObject();
    }
    
}
