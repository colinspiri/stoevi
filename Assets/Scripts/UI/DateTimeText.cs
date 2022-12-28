using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DateTimeText : MonoBehaviour {
    private TextMeshProUGUI text;
    
    // constants
    // public int startingHour;
    
    // state
    private bool showNightPrompt;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        showNightPrompt = false;
        UpdateText(0);
        DayManager.OnNight += () => {
            showNightPrompt = true;
        };
    }

    private void OnEnable() {
        DayManager.OnSecondTick += UpdateText;
    }

    private void OnDisable() {
        DayManager.OnSecondTick -= UpdateText;
    }

    private void UpdateText(int secondsElapsed) {
        string uiText = "Day " + PlayerPrefs.GetInt("CurrentDay", 1);
        
        // float additionalSeconds = startingHour * 60f;
        uiText += "\n" + Util.FormatTimer(secondsElapsed);

        if (showNightPrompt) {
            uiText += "\n";
            uiText += "return to gate to go home";
        }

        text.text = uiText;
    }
}
