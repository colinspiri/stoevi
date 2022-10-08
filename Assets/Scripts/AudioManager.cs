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
    // UI
    public AudioSource backSound;
    public AudioSource selectSound;
    public AudioSource submitSound;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else {
            Destroy(gameObject);
            return;
        }
    }

    private void Start() {
        backSound.ignoreListenerPause = true;
        selectSound.ignoreListenerPause = true;
        submitSound.ignoreListenerPause = true;
    }

    public void StopGameSound() {
        AudioListener.pause = true;
    }
    public void ResumeGameSound() {
        AudioListener.pause = false;
    }

    private void Update() {
        if (FirstPersonController.Instance) {
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
    }

    public void PlayWaterSound() { waterSound.Play(); }
    public void PlayHarvestSound() { harvestSound.Play(); }
    public void PlaySearchSound() { searchSound.Play(); }
    public void PlayChaseSound() { chaseSound.Play(); }

    public void PlayBackSound() {
        backSound.Play();
    }

    public void PlaySelectSound() {
        selectSound.Play();
    }
    public void PlaySubmitSound() { submitSound.Play(); }
}
