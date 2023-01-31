using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SensitivitySlider : MonoBehaviour {
    private Slider slider;
    public FloatReference defaultSensitivity;

    private void Awake() {
        slider = GetComponent<Slider>();
    }

    // Start is called before the first frame update
    void Start() {
        slider.onValueChanged.AddListener(value => {
            if (InputHandler.Instance) InputHandler.Instance.sensitivity = value;
            PlayerPrefs.SetFloat("Sensitivity", slider.value);
        });
        if (InputHandler.Instance) InputHandler.Instance.sensitivity = slider.value;
    }

    private void OnEnable() {
        slider.value = PlayerPrefs.GetFloat("Sensitivity", defaultSensitivity);
    }
}