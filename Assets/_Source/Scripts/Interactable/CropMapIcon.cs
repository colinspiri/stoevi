using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CropMapIcon : MonoBehaviour {
    // components
    public GameObject tomato;
    public GameObject sproutHealthy;
    public GameObject sproutWilted;
    public GameObject intermediateHealthy;
    public GameObject intermediateWilted;
    public GameObject unripeHealthy;
    public GameObject unripeWilted;
    public Slider timerPanel;

    public void UpdateMapIcon(Crop.GrowthStage stage, Crop.State state,  Crop.Health health) {
        // disable all icons
        tomato.SetActive(false);
        sproutHealthy.SetActive(false);
        sproutWilted.SetActive(false);
        intermediateHealthy.SetActive(false);
        intermediateWilted.SetActive(false);
        unripeHealthy.SetActive(false);
        unripeWilted.SetActive(false);
        
        // enable correct icon
        if (stage == Crop.GrowthStage.Ripe) {
           tomato.SetActive(true);
        }
        else if (stage == Crop.GrowthStage.Sprout) {
            if(health == Crop.Health.Wilted || health == Crop.Health.Dead) sproutWilted.SetActive(true);
            else sproutHealthy.SetActive(true);
        }
        else if (stage == Crop.GrowthStage.Intermediate) {
            if(health == Crop.Health.Wilted || health == Crop.Health.Dead) intermediateWilted.SetActive(true);
            else intermediateHealthy.SetActive(true);
        }
        else if (stage == Crop.GrowthStage.Unripe) {
            if(health == Crop.Health.Wilted || health == Crop.Health.Dead) unripeWilted.SetActive(true);
            else unripeHealthy.SetActive(true);
        }
        else if (stage == Crop.GrowthStage.Bare) {
            unripeWilted.SetActive(true);
        }
        
        // set icon
        if (state == Crop.State.Growing) {
            timerPanel.gameObject.SetActive(true);
        }
        else {
            timerPanel.gameObject.SetActive(false);
        }
        
        // set rotation
        transform.rotation = Quaternion.Euler(-90, 180, 0);
    }

    public void UpdateTimer(float value) {
        timerPanel.value = value;
    }
}
