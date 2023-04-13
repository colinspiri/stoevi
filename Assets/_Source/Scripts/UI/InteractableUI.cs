using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractableUI : MonoBehaviour {
    // components
    
    [Header("Object Info")]
    public GameObject objectInfo;
    public TextMeshProUGUI objectName;
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
        objectInfo.SetActive(false);
        progressSlider.value = 0;
        buttonPrompt.text = "";
        
        // timer
        timerPanel.gameObject.SetActive(false);
    }

    private void ShowSelectedObject() {
        var selectedObject = InteractableManager.Instance.selectedObject;
        
        ShowObjectInfo(selectedObject);

        progressSlider.value = 0;

        buttonPrompt.gameObject.SetActive(true);
        buttonPrompt.alpha = 1;
        buttonPrompt.text = selectedObject.GetButtonPromptPrimary();
        string secondary = selectedObject.GetButtonPromptSecondary();
        if (!secondary.Equals("")) {
            if (selectedObject.GetButtonPromptPrimary().Equals("") == false) buttonPrompt.text += "\n";
            buttonPrompt.text += secondary;
        }
        
        ShowTimer(selectedObject);
    }

    private void ShowInteractingObject() {
        var selectedObject = InteractableManager.Instance.selectedObject;

        ShowObjectInfo(selectedObject);
        
        progressSlider.value = InteractableManager.Instance.GetInteractingFloat();

        buttonPrompt.gameObject.SetActive(true);
        buttonPrompt.alpha = 0.5f;
        buttonPrompt.text = selectedObject.GetButtonPromptPrimary();
        string secondary = selectedObject.GetButtonPromptSecondary();
        if (!secondary.Equals("")) {
            if (selectedObject.GetButtonPromptPrimary().Equals("") == false) buttonPrompt.text += "\n";
            buttonPrompt.text += secondary;
        }
        
        ShowTimer(selectedObject);
    }

    private void ShowObjectInfo(Interactable selectedObject) {
        objectInfo.SetActive(true);
        objectName.text = selectedObject.GetObjectName();
        if (selectedObject.GetObjectDescription() != "") {
            // objectInfoLine.SetActive(true);
            objectDescription.gameObject.SetActive(true);
            objectDescription.text = selectedObject.GetObjectDescription();
        }
        else {
            // objectInfoLine.SetActive(false);
            objectDescription.gameObject.SetActive(false);
        }
    }

    private void ShowTimer(Interactable selectedObject) {
        var value = selectedObject.GetTimerValue();
        if (value != 0 && selectedObject.GetTimerIcon() == TimerIcon.Growth || selectedObject.GetTimerIcon() == TimerIcon.Ripe) {
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
