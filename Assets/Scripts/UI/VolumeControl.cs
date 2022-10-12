using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeControl : MonoBehaviour {
    public string mixerChannel;

    private Slider slider;

    private void Awake() {
        slider = GetComponent<Slider>();
    }

    // Start is called before the first frame update
    void Start() {
        slider.onValueChanged.AddListener(value => {
            AudioManager.Instance.SetVolume(mixerChannel, value);
        });
        slider.value = PlayerPrefs.GetFloat(mixerChannel, slider.value);
    }

    public void PlaySFXTestSound()
    {
        if(AudioManager.Instance) AudioManager.Instance.PlaySelectSound();
    }

    private void OnEnable() {
        slider.value = PlayerPrefs.GetFloat(mixerChannel, slider.value);
    }

    private void OnDisable() {
        PlayerPrefs.SetFloat(mixerChannel, slider.value);
    }
}