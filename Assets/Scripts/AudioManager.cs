using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;
    
    // sfx
    public AudioSource walkingSound;
    public AudioSource runningSound;
    public AudioSource waterSound;
    public AudioSource harvestSound;
    public AudioSource searchSound;
    public AudioSource chaseSound;

    private void Awake() {
        Instance = this;
    }

    public void StopGameSound() {
        AudioListener.pause = true;
    }
    public void ResumeGameSound() {
        AudioListener.pause = false;
    }

    private void Update() {
        var state = FirstPersonController.Instance.GetMoveState;
        if (state == FirstPersonController.MoveState.Still || state == FirstPersonController.MoveState.CrouchWalking) {
            walkingSound.Stop();
            runningSound.Stop();
        }
        else if (state == FirstPersonController.MoveState.Walking) {
            if (!walkingSound.isPlaying) walkingSound.Play();
            runningSound.Stop();
        }
        else if (state == FirstPersonController.MoveState.Running) {
            walkingSound.Stop();
            if(!runningSound.isPlaying) runningSound.Play();
        }
    }

    public void PlayWaterSound() { waterSound.Play(); }
    public void PlayHarvestSound() { harvestSound.Play(); }
    public void PlaySearchSound() { searchSound.Play(); }
    public void PlayChaseSound() { chaseSound.Play(); }
}
