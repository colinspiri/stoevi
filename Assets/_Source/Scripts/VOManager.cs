using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Yarn;
using Yarn.Unity;
using Random = UnityEngine.Random;

public class VOManager : MonoBehaviour {
    // components
    private AudioSource audioSource;
    public TextMeshProUGUI speakerText;

    // data
    public List<AudioClip> voDimo;
    public List<AudioClip> voRumen;
    public List<AudioClip> voYoan;
    public List<AudioClip> voAna;
    
    // state
    private int previousRandomIndex;

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        previousRandomIndex = -1;
    }

    public void TryPlayVO() {
        // check if already playing
        if (audioSource.isPlaying) return;
        
        // check current speaker
        string speaker = speakerText.text;
        List<AudioClip> voClips;
        if (speaker.Contains("Dimo")) voClips = voDimo;
        else if (speaker.Contains("Rumen")) voClips = voRumen;
        else if (speaker.Contains("Yoan")) voClips = voYoan;
        else if (speaker.Contains("Ana")) voClips = voAna;
        else voClips = new List<AudioClip>();
        if (voClips.Count == 0) return;

        // play random clip
        int randomIndex;
        do {
            randomIndex = Random.Range(0, voClips.Count);
        } while (randomIndex == previousRandomIndex);
        previousRandomIndex = randomIndex;

        // play clip
        AudioClip randomClip = voClips[randomIndex];
        audioSource.clip = randomClip;
        audioSource.Play();
    }
}
