using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour {
    // constants
    public float minutesUntilEvening;
    public float minutesUntilDark;
    private float timeUntilEvening;
    private float timeUntilDark;
    
    // state
    [SerializeField] private float timer;
    
    // callbacks
    public static event Action<int> OnSecondTick = delegate { };
    public static event Action OnNight = delegate { };

    // Start is called before the first frame update
    void Start() {
        timeUntilEvening = minutesUntilEvening * 60;
        timeUntilDark = minutesUntilDark * 60;
        timer = 0;
        LightManager.Instance.UpdateLighting(0);
    }

    // Update is called once per frame
    void Update() {
        float beforeTimer = timer;
        timer += Time.deltaTime;

        // if seconds incremented this frame
        if(Mathf.Floor(timer) > Mathf.Floor(beforeTimer)) OnSecondTick(Mathf.FloorToInt(timer));
        
        // update lighting
        if (timer >= timeUntilEvening + timeUntilDark) {
            LightManager.Instance.UpdateLighting(1);
            OnNight();
        }
        else if (timer >= timeUntilEvening) {
            float lightPercent = (timer - timeUntilEvening) / timeUntilDark;
            LightManager.Instance.UpdateLighting(lightPercent);
        }
    }
}
