using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn;
using Yarn.Unity;
using Random = UnityEngine.Random;

public class VOManager : MonoBehaviour {
    // components
    private AudioSource audioSource;
    
    // data
    public List<AudioClip> voClips;
    
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
        
        // TODO check current character
        
        // TODO if in game, apply radio filter
        
        // play random clip
        int randomIndex;
        do {
            randomIndex = Random.Range(0, voClips.Count);
        } while (randomIndex == previousRandomIndex);
        previousRandomIndex = randomIndex;
        Debug.Log("playing clip " + randomIndex);

        // play clip
        AudioClip randomClip = voClips[randomIndex];
        audioSource.clip = randomClip;
        audioSource.Play();
    }
}
