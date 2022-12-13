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
    public GameObject objectInfo;
    public TextMeshProUGUI objectName;
    public GameObject objectInfoLine;
    public TextMeshProUGUI objectDescription;
    
    public Slider progressSlider;

    public TextMeshProUGUI buttonPrompt;
    

    private void Start() {
        HideInteractableUI();
    }
    
    private void Update() {
        // TODO: refactor so that each interactable has UpdateUI() function to call when values change
        
        switch (InteractableManager.Instance.interactionState)
        {
            case InteractableManager.InteractionState.None:
                if (ResourceManager.Instance.carryingFertilizer) {
                    objectInfo.SetActive(false);
                    progressSlider.value = 0;
                    buttonPrompt.gameObject.SetActive(true);
                    buttonPrompt.text = "E to drop fertilizer";
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
        progressSlider.value = 0;
        objectInfo.SetActive(false);
        buttonPrompt.text = "";
    }

    private void ShowSelectedObject() {
        var selectedObject = InteractableManager.Instance.SelectedObject;
        
        objectInfo.SetActive(true);
        objectName.text = selectedObject.GetObjectName();
        if (selectedObject.GetObjectDescription() != "") {
            objectInfoLine.SetActive(true);
            objectDescription.gameObject.SetActive(true);
            objectDescription.text = selectedObject.GetObjectDescription();
        }
        else {
            objectInfoLine.SetActive(false);
            objectDescription.gameObject.SetActive(false);
        }

        progressSlider.value = 0;

        buttonPrompt.gameObject.SetActive(true);
        buttonPrompt.text = selectedObject.GetButtonPrompt();
        buttonPrompt.alpha = 1;
    }

    private void ShowInteractingObject() {
        var selectedObject = InteractableManager.Instance.SelectedObject;

        objectInfo.SetActive(true);
        objectName.text = selectedObject.GetObjectName();
        if (selectedObject.GetObjectDescription() != "") {
            objectInfoLine.SetActive(true);
            objectDescription.gameObject.SetActive(true);
            objectDescription.text = selectedObject.GetObjectDescription();
        }
        else {
            objectInfoLine.SetActive(false);
            objectDescription.gameObject.SetActive(false);
        }
        progressSlider.value = InteractableManager.Instance.GetInteractingFloat();

        buttonPrompt.gameObject.SetActive(true);
        buttonPrompt.text = selectedObject.GetButtonPrompt();
        buttonPrompt.alpha = 0.5f;
    }
}
