using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WaterUI : MonoBehaviour
{
    // components
    public Slider waterSlider;
    
    // constants
    public float valueLerpTime;

    // Start is called before the first frame update
    void Start() {
        waterSlider.value = 1;
        
        if(ResourceManager.Instance) ResourceManager.Instance.onWaterValueChange.AddListener(waterValue => {
            waterSlider.DOValue(waterValue, valueLerpTime);
        });
    }
    
    
}
