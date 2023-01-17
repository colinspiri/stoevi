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
    public HeldItem heldItem;
    
    [Header("Object Info")]
    public GameObject objectInfo;
    public TextMeshProUGUI objectName;
    public GameObject objectInfoLine;
    public TextMeshProUGUI objectDescription;
    
    [Header("Progress Slider")]
    public Slider progressSlider;

    [Header("Button Prompt")]
    public TextMeshProUGUI buttonPrompt;

    [Header("Timer Panel")] 
    public GameObject timerPanel;
    public Slider timerSlider;
    public GameObject timerWaterIcon;
    public GameObject timerGrowthIcon;
    public GameObject timerRipeIcon;
    public TextMeshProUGUI timerText;
    
    // constants
    public enum TimerIcon { None, Water, Growth, Ripe }

    private void Start() {
        HideInteractableUI();
    }
    
    private void Update() {
        UpdateUI();
    }

    private void UpdateUI() {
        if (heldItem.HoldingItem()) {
            objectInfo.SetActive(false);
            progressSlider.value = 0;
            buttonPrompt.gameObject.SetActive(true);
            buttonPrompt.text = Interactable.GetInteractButton() + " to drop " + heldItem.heldItem.itemName;
            return;
        }
        
        switch (InteractableManager.Instance.interactionState)
        {
            case InteractableManager.InteractionState.None:
                HideInteractableUI();
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
        
        // timer
        timerPanel.gameObject.SetActive(false);
    }

    private void ShowSelectedObject() {
        var selectedObject = InteractableManager.Instance.selectedObject;
        
        ShowObjectInfo(selectedObject);

        progressSlider.value = 0;

        buttonPrompt.gameObject.SetActive(true);
        buttonPrompt.text = selectedObject.GetButtonPrompt();
        buttonPrompt.alpha = 1;
        
        ShowTimer(selectedObject);
    }

    private void ShowInteractingObject() {
        var selectedObject = InteractableManager.Instance.selectedObject;

        ShowObjectInfo(selectedObject);
        
        progressSlider.value = InteractableManager.Instance.GetInteractingFloat();

        buttonPrompt.gameObject.SetActive(true);
        buttonPrompt.text = selectedObject.GetButtonPrompt();
        buttonPrompt.alpha = 0.5f;
        
        ShowTimer(selectedObject);
    }

    private void ShowObjectInfo(Interactable selectedObject) {
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
    }

    private void ShowTimer(Interactable selectedObject) {
        var value = selectedObject.GetTimerValue();
        if (value != 0) {
            timerPanel.SetActive(true);
            timerSlider.value = value;
            
            // icon
            TimerIcon iconType = selectedObject.GetTimerIcon();
            timerWaterIcon.SetActive(iconType == TimerIcon.Water);
            timerGrowthIcon.SetActive(iconType == TimerIcon.Growth);
            timerRipeIcon.SetActive(iconType == TimerIcon.Ripe);
            
            // text
            timerText.text = Util.FormatTimer(selectedObject.GetTimerTime());
        }
        else timerPanel.SetActive(false);
    }
}
