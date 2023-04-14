using System;
using System.Collections;
using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;

public class TorbalanHuff : MonoBehaviour {
    public AudioSource audioSource;
    public AudioClip huff1;
    public AudioClip huff2;
    public AudioClip searchHuff;
    public AudioClip chaseHuff;
    
    // Start is called before the first frame update
    void Start()
    {
        TorbalanStateTracker.Instance.onStateChange.AddListener(OnTorbalanStateChange);
    }

    private void OnDisable() {
        TorbalanStateTracker.Instance.onStateChange.RemoveListener(OnTorbalanStateChange);
    }

    public void PlayHuff1() {
        TryPlayClip(huff1);
    }

    public void PlayHuff2() {
        TryPlayClip(huff2);
    }

    private void OnTorbalanStateChange(TorbalanStateTracker.TorbalanState state) {
        if (state == TorbalanStateTracker.TorbalanState.Search) {
            TryPlayClip(searchHuff);
        }
        else if (state == TorbalanStateTracker.TorbalanState.Chase) {
            TryPlayClip(chaseHuff);
        }
    }
    
    private void TryPlayClip(AudioClip clip) {
        if (!audioSource.isPlaying) {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
