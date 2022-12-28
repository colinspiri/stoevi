using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SensitivitySlider : MonoBehaviour {
    private Slider slider;

    private void Awake() {
        slider = GetComponent<Slider>();
    }

    // Start is called before the first frame update
    void Start() {
        slider.onValueChanged.AddListener(value => {
            if (InputHandler.Instance) InputHandler.Instance.sensitivity = value;
        });
        slider.value = PlayerPrefs.GetFloat("Sensitivity", slider.value);
        if (InputHandler.Instance) InputHandler.Instance.sensitivity = slider.value;
    }

    private void OnEnable() {
        slider.value = PlayerPrefs.GetFloat("Sensitivity", slider.value);
    }

    private void OnDisable() {
        PlayerPrefs.SetFloat("Sensitivity", slider.value);
    }
}