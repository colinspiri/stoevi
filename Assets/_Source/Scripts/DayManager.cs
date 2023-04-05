using System;
using UnityEngine;

public class DayManager : MonoBehaviour {
    public int day;
    public TimeOfDay timeOfDay;

    private void Awake() {
#if UNITY_EDITOR
        PlayerPrefs.SetInt("CurrentDay", day);
#endif
    }

    // Start is called before the first frame update
    void Start() {
        timeOfDay.ResetCurrentTime();
    }

    // Update is called once per frame
    void Update() {
        timeOfDay.AdvanceCurrentTime();
    }
}
