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
    private GameObject yoanSheep;
    
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        objectiveComplete.SetValue(0);
        
        yoanSheep = GameObject.Find("Yoan-Sheep");
    }

    // Update is called once per frame
    void Update() {
        int day = PlayerPrefs.GetInt("CurrentDay", 1);
        
        // day 5
        if (day == 5) {
            if (objectiveComplete.Value == 1) objectiveText = "Go home";
            else {
                objectiveText = "Find Yoan";
                float distanceToYoan = Vector3.Distance(FirstPersonMovement.Instance.transform.position,
                    yoanSheep.transform.position);
                int displayDistance = 10 * Mathf.RoundToInt(distanceToYoan / 10);
                if (displayDistance <= 70) {
                    objectiveText += "\ndistance: " + Mathf.RoundToInt(displayDistance) + "m";
                } 
            }
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
        else {
            objectiveComplete.SetValue(1);

            if (DayManager.Instance != null && DayManager.Instance.timeOfDay.IsNight() == false) {
                objectiveText = "Farm until night";
            }
            else {
                objectiveText = "Return to gate to go home";
            }
        }
        
    }
}
