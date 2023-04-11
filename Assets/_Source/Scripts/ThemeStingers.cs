using System;
using System.Collections;
using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;
using Yarn.Unity;

public class ThemeStingers : MonoBehaviour {
    private static ThemeStingers Instance;
    private AudioSource audioSource;
    
    public List<AudioClip> themeStingers;

    private void Awake() {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    [YarnCommand("theme")]
    public static void PlayTheme(int number) {
        int index = number - 1;
        if (index < 0 || index >= Instance.themeStingers.Count) {
            Debug.Log("theme " + number + " is out of range");
        }
        
        Debug.Log("Playing theme stinger " + number);

        AudioClip clip = Instance.themeStingers[index];
        Instance.audioSource.clip = clip;
        Instance.audioSource.Play();
    }
}
