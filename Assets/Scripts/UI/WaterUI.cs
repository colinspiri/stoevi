using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WaterUI : MonoBehaviour
{
    // components
    public static WaterUI Instance;
    public Slider waterSlider;
    
    // public constants
    public int maxWater;
    
    // state
    private int currentWater;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        currentWater = maxWater;
        waterSlider.value = 1;
    }

    public void UseWater() {
        currentWater--;
        UpdateSlider();
    }

    public void RefillWater() {
        currentWater = maxWater;
        UpdateSlider();
    }

    public bool IsWaterEmpty() {
        return currentWater <= 0;
    }
    public bool IsWaterFull() {
        return currentWater == maxWater;
    }

    private void UpdateSlider() {
        var value = (float)currentWater / maxWater;
        waterSlider.DOValue(value, 0.5f);
    }
}
