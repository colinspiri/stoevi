using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Yarn.Unity;

[CreateAssetMenu(fileName = "AudioSettings", menuName = "AudioSettings")]
public class AudioSettings : ScriptableObject {
    // components
    public AudioMixer mixer;
    
    // constants
    public SerializedDictionary<string, float> defaultMixerChannelVolume;
    
    // state
    
    
    public void Initialize() {
        // set initial volume
        foreach (var pair in defaultMixerChannelVolume) {
            var channel = pair.Key;
            SetVolume(channel, GetVolume(channel));
        }
    }

    public void ChangeVolume(string mixerChannel, float value) {
        SetVolume(mixerChannel, value);
        PlayerPrefs.SetFloat(mixerChannel, value);
    }
    
    public float GetVolume(string mixerChannel) {
        return PlayerPrefs.GetFloat(mixerChannel, defaultMixerChannelVolume[mixerChannel]);
    }

    private void SetVolume(string mixerChannel, float value) {
        mixer.SetFloat(mixerChannel, Mathf.Log10(value) * 20f);
    }
}