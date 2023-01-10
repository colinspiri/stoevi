using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeControl : MonoBehaviour {
    // components
    private Slider slider;
    public AudioSettings audioSettings;

    // constants
    public string mixerChannel;

    private void Awake() {
        slider = GetComponent<Slider>();
    }

    // Start is called before the first frame update
    void Start() {
        slider.onValueChanged.AddListener(value => {
            audioSettings.ChangeVolume(mixerChannel, value);
        });
    }

    public void PlaySFXTestSound() {
        if(AudioManager.Instance) AudioManager.Instance.PlaySelectSound();
    }

    private void OnEnable() {
        slider.value = audioSettings.GetVolume(mixerChannel);
    }
}