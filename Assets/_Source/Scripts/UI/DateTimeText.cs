using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DateTimeText : MonoBehaviour {
    private TextMeshProUGUI text;

    public List<TimeOfDay> timeOfDays;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        // get correct timeOfDay
        int currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        int index = currentDay - 1;
        if (index >= timeOfDays.Count) index = timeOfDays.Count - 1;
        TimeOfDay timeOfDay = timeOfDays[index];
        
        // format text
        string uiText = "Day " + currentDay;
        uiText += "\n" + Util.FormatTimer(timeOfDay.SecondsElapsed);

        // return to gate prompt
        /*if (timeOfDay.IsNight()) {
            uiText += "\n";
            uiText += "return to gate to go home";
        }*/

        text.text = uiText;
    }
}
