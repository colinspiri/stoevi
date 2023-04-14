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
        if (day == 1) {
            if(playerTomatoes.Value >= requiredTomatoesDay1.Value) objectiveComplete.SetValue(1);
            objectiveText = "Harvest " + playerTomatoes.Value + "/" + requiredTomatoesDay1.Value + " tomatoes";
        }
        else if (day == 2) {
            if(playerTomatoes.Value >= requiredTomatoesDay2.Value) objectiveComplete.SetValue(1);
            objectiveText = "Harvest " + playerTomatoes.Value + "/" + requiredTomatoesDay2.Value + " tomatoes";
        }
        else if (day == 3) {
            if(playerTomatoes.Value >= requiredTomatoesDay3.Value) objectiveComplete.SetValue(1);
            objectiveText = "Harvest " + playerTomatoes.Value + "/" + requiredTomatoesDay3.Value + " tomatoes";
        }
        else if (day == 4) {
            if(playerTomatoes.Value >= requiredTomatoesDay4.Value) objectiveComplete.SetValue(1);
            objectiveText = "Harvest " + playerTomatoes.Value + "/" + requiredTomatoesDay4.Value + " tomatoes";
        }
        else if (day == 5) {
            // TODO if yoan has been leashed
            objectiveText = "Find Yoan";
        }
    }
}
