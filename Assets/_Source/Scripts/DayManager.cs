using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour {
    public TimeOfDay timeOfDay;

    // Start is called before the first frame update
    void Start() {
        timeOfDay.ResetCurrentTime();
    }

    // Update is called once per frame
    void Update() {
        timeOfDay.AdvanceCurrentTime();
    }
}
