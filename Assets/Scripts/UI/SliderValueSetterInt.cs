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
        slider.value = GetSliderValue();
    }

    private void Update() {
        if (previousValue != currentValue.Value) {
            LerpSliderValue();
            previousValue = currentValue.Value;
        }
    }

    private void LerpSliderValue() {
        float sliderValue = GetSliderValue();
        slider.DOValue(sliderValue, sliderLerpTime);
    }

    private float GetSliderValue() {
        return Mathf.Clamp01(Mathf.InverseLerp(minValue.Value, maxValue.Value, currentValue.Value));
    }
}