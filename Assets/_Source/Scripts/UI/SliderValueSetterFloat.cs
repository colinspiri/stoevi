using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueSetterFloat : MonoBehaviour
{
    // components
    public Slider slider;
    public FloatReference currentValue;
    public FloatReference maxValue;
    public FloatReference minValue;

    // constants
    public float sliderLerpTime;
    
    // state
    private float previousValue = -1;

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
        float targetValue = GetSliderValue();
        if (sliderLerpTime > 0) slider.DOValue(targetValue, sliderLerpTime);
        else slider.value = targetValue;
    }

    private float GetSliderValue() {
        return Mathf.Clamp01(Mathf.InverseLerp(minValue.Value, maxValue.Value, currentValue.Value));
    }
}