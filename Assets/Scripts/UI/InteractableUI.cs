using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InteractableUI : MonoBehaviour {
    // components
    public TextMeshProUGUI interactableText;
    public Slider ripenTimer;
    
    // state
    private Crop currentRipeningCrop;
    

    private void Start() {
        ripenTimer.gameObject.SetActive(false);
        if (InteractableManager.Instance != null) {
            InteractableManager.Instance.onSelectedObjectChange.AddListener(ShowSelectedObject);
        }
    }

    private void ShowSelectedObject(Interactable selectedObject) {
        if (selectedObject == null) {
            interactableText.text = "";
            ripenTimer.gameObject.SetActive(false);
            return;
        }
        
        interactableText.text = selectedObject.GetUIText();

        // show ripen timer if crop is ripening
        if (selectedObject is Crop { stage: Crop.CropStage.Ripening } crop) {
            ripenTimer.gameObject.SetActive(true);
            currentRipeningCrop = crop;
        }
    }

    private void Update() {
        if (currentRipeningCrop != null) {
            ripenTimer.value = currentRipeningCrop.GetRipenTimeFloat();
        }
    }
}
