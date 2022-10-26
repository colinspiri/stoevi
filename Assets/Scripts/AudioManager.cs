using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;
    public AudioMixer mixer;

    public SceneReference mainMenuScene;
    
    // music
    public AudioSource mainMenuMusic;
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
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start() {
        backSound.ignoreListenerPause = true;
        selectSound.ignoreListenerPause = true;
        submitSound.ignoreListenerPause = true;

        SceneManager.activeSceneChanged += (oldScene, newScene) => PlaySoundsOnSceneStart(newScene);
        PlaySoundsOnSceneStart(SceneManager.GetActiveScene());
    }

    private void PlaySoundsOnSceneStart(Scene newScene) {
        ResumeGameSound();
        // stop all sounds
        // mainMenuMusic.Stop();
        
        // play sounds based on new scene
        if (newScene.path == mainMenuScene.ScenePath) {
            mainMenuMusic.Play();
        }
        else mainMenuMusic.Stop();
    }

    public void StopGameSound() {
        AudioListener.pause = true;
    }
    public void ResumeGameSound() {
        AudioListener.pause = false;
    }
    
    public void SetVolume(string mixerChannel, float value) {
        mixer.SetFloat(mixerChannel, Mathf.Log10(value) * 20f);
        PlayerPrefs.SetFloat(mixerChannel, value);
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
