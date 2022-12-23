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

        InteractableManager.OnSelectObject += interactable => {
        };
    }
    
    private void Update() {
        // TODO: refactor so that each interactable has UpdateUI() function to call when values change
        
        switch (InteractableManager.Instance.interactionState)
        {
            case InteractableManager.InteractionState.None:
                // Debug.Log("InteractableUI: none " + ResourceManager.Instance.carryingFertilizer);
                if (ResourceManager.Instance && ResourceManager.Instance.carryingFertilizer) {
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
        
        // timer
        timerPanel.gameObject.SetActive(false);
    }

    private void ShowSelectedObject() {
        var selectedObject = InteractableManager.Instance.selectedObject;
        
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
        
        ShowTimer();
    }

    private void ShowInteractingObject() {
        var selectedObject = InteractableManager.Instance.selectedObject;

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
        
        ShowTimer();
    }

    private void ShowTimer() {
        var selectedObject = InteractableManager.Instance.selectedObject;
        
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
            timerText.text = FormatTimer(selectedObject.GetTimerTime());
        }
        else timerPanel.SetActive(false);
    }
    
    private string FormatTimer(float timer) {
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
