using System;
using UnityEngine;

public class ObjectiveManager : MonoBehaviour {
    // components
    public static ObjectiveManager Instance;
    public IntReference playerTomatoes;
    
    // constants
    public IntReference requiredTomatoesDay1;
    public IntReference requiredTomatoesDay2;
    public IntReference requiredTomatoesDay3;
    public IntReference requiredTomatoesDay4;

    // state
    public IntVariable objectiveComplete;
    public string objectiveText { get; private set; }
    
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        objectiveComplete.SetValue(0);
    }

    // Update is called once per frame
    void Update() {
        int day = PlayerPrefs.GetInt("CurrentDay", 1);
        
        // day 5
        if (day == 5) {
            objectiveText = "Find Yoan";
            return;
        }
        
        // days 1-4
        int requiredTomatoes = day switch {
            1 => requiredTomatoesDay1.Value,
            2 => requiredTomatoesDay2.Value,
            3 => requiredTomatoesDay3.Value,
            4 => requiredTomatoesDay4.Value,
            _ => 0
        };
        
        if (playerTomatoes.Value < requiredTomatoes) {
            objectiveText = "Harvest " + playerTomatoes.Value + "/" + requiredTomatoes + " tomatoes";
        }
        else if (DayManager.Instance != null && DayManager.Instance.timeOfDay.IsNight() == false) {
            objectiveText = "Farm until night";
        }
        else {
            objectiveComplete.SetValue(1);
            objectiveText = "Return to gate to go home";
        }
    }
}
