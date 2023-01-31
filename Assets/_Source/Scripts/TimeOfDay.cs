using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeOfDay", menuName = "TimeOfDay")]
public class TimeOfDay : ScriptableObject {
    // constants
    public float eveningTimeInMinutes;
    private float EveningTime => eveningTimeInMinutes * 60f;
    public float nightTimeInMinutes;
    private float NightTime => nightTimeInMinutes * 60f;
    
    // state
    public float currentTime;
    public int SecondsElapsed => Mathf.FloorToInt(currentTime);
    
    // events
    public GameEvent onNight;

    public bool IsNight() {
        return currentTime >= NightTime;
    }
    public float GetTimePercent() {
        if (currentTime < EveningTime) {
            return 0;
        }

        if (currentTime >= NightTime) {
            return 1;
        }

        return (currentTime - EveningTime) / (NightTime - EveningTime);
    }

    public void ResetCurrentTime() {
        currentTime = 0;
    }
    public void AdvanceCurrentTime() {
        bool wasNight = IsNight();
        
        currentTime += Time.deltaTime;

        if (IsNight() && !wasNight) {
            onNight.Raise();
        } 
    }

    public void SetEvening() {
        currentTime = EveningTime;
    }
    public void SetNight(bool callEvent = true) {
        currentTime = NightTime;
        if(callEvent) onNight.Raise();
    }
}