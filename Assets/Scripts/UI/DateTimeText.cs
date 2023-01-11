using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DateTimeText : MonoBehaviour {
    private TextMeshProUGUI text;
    public TimeOfDay timeOfDay;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        string uiText = "Day " + PlayerPrefs.GetInt("CurrentDay", 1);
        
        uiText += "\n" + Util.FormatTimer(timeOfDay.SecondsElapsed);

        if (timeOfDay.IsNight()) {
            uiText += "\n";
            uiText += "return to gate to go home";
        }

        text.text = uiText;
    }
}
