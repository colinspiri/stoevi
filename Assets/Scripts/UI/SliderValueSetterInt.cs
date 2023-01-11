using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueSetterInt : MonoBehaviour
{
    // components
    public Slider slider;
    public IntReference currentValue;
    public IntReference maxValue;
    public IntReference minValue;

    // constants
    public float sliderLerpTime;
    
    // state
    private int previousValue = -1;

    private void OnEnable() {
        LerpSliderValue();
    }

    private void Update() {
        if (previousValue != currentValue.Value) {
            LerpSliderValue();
            previousValue = currentValue.Value;
        }
    }

    private void LerpSliderValue() {
        float sliderValue = Mathf.Clamp01(Mathf.InverseLerp(minValue.Value, maxValue.Value, currentValue.Value));
        slider.DOValue(sliderValue, sliderLerpTime);
    }
}