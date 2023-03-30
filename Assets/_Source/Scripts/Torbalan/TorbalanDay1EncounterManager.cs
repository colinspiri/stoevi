using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorbalanDay1EncounterManager : MonoBehaviour {
    public bool Encounter { get; set; }
    
    public List<float> encounterTimesInMinutes;

    private int encounterIndex;
    private float timer;

    // Update is called once per frame
    void Update() {
        if (encounterIndex >= encounterTimesInMinutes.Count) return;

        if (timer >= encounterTimesInMinutes[encounterIndex] * 60f) {
            Encounter = true;
        }
        else {
            timer += Time.deltaTime;
        }
    }

    public void EndEncounter() {
        Encounter = false;
        encounterIndex++;
    }
}
