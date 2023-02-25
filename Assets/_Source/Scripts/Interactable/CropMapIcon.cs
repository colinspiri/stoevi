using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CropMapIcon : MonoBehaviour {
    // components
    public Image mapIcon;
    public Slider timerPanel;
    public GameObject growthIcon;
    public GameObject ripeIcon;
    
    // constants
    public Color defaultColor;
    public Color ripeColor;

    public void UpdateMapIcon(Crop.GrowthStage stage, Crop.State state) {
        if (stage == Crop.GrowthStage.Ripe) {
            mapIcon.color = ripeColor;
        }
        else {
            mapIcon.color = defaultColor;
        }
        
        // set icon
        if (state == Crop.State.Growing) {
            timerPanel.gameObject.SetActive(true);

            if(stage == Crop.GrowthStage.Unripe) ripeIcon.SetActive(true);
            else growthIcon.SetActive(true);
        }
        else {
            timerPanel.gameObject.SetActive(false);
            ripeIcon.SetActive(false);
            growthIcon.SetActive(false);
        }
    }

    public void UpdateTimer(float value) {
        timerPanel.value = value;
    }
}
