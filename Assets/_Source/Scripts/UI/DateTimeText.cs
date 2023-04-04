using TMPro;
using UnityEngine;

public class DateTimeText : MonoBehaviour {
    private TextMeshProUGUI text;

    public TimeOfDay timeOfDay_Day1;
    public TimeOfDay timeOfDay_Day2;

    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update() {
        // get correct timeOfDay
        int currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        TimeOfDay timeOfDay = timeOfDay_Day1;
        if (currentDay == 1) timeOfDay = timeOfDay_Day1;
        else timeOfDay = timeOfDay_Day2;
        
        // format text
        string uiText = "Day " + currentDay;
        uiText += "\n" + Util.FormatTimer(timeOfDay.SecondsElapsed);

        // return to gate prompt
        if (timeOfDay.IsNight()) {
            uiText += "\n";
            uiText += "return to gate to go home";
        }

        text.text = uiText;
    }
}
