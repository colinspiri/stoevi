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
}
