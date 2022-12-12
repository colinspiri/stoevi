using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SpookuleleAudio;
using StarterAssets;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;
    public AudioMixer mixer;

    public SceneReference mainMenuScene;
    
    [Header("Music")]
    public AudioSource mainMenuMusic;
    public ASoundContainer detectedStinger;
    public ASoundContainer chaseStinger;

    [Header("Ambience")] 
    public AudioSource farmAmbience;

    [Header("SFX")] 
    public ASoundContainer waterSoundContainer;
    public AudioSource walkingSound;
    public AudioSource runningSound;
    public ASoundContainer harvestSound;

    [Header("UI")]
    public ASoundContainer backSound;
    public ASoundContainer selectSound;
    public ASoundContainer submitSound;

    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start() {
        farmAmbience.ignoreListenerPause = true;
        /*backSound.ignoreListenerPause = true;
        selectSound.ignoreListenerPause = true;
        submitSound.ignoreListenerPause = true;*/

        SceneManager.activeSceneChanged += (oldScene, newScene) => PlaySoundsOnSceneStart(newScene);
        PlaySoundsOnSceneStart(SceneManager.GetActiveScene());
    }

    private void PlaySoundsOnSceneStart(Scene newScene) {
        AudioListener.pause = false;
        // stop all sounds
        
        // play sounds based on new scene
        if (newScene.path == mainMenuScene.ScenePath) {
            mainMenuMusic.Play();
        }
    }

    public void PauseGameSound() {
        AudioListener.pause = true;
        farmAmbience.DOFade(0.2f, 1).SetUpdate(true);
    }
    public void ResumeGameSound() {
        AudioListener.pause = false;
        farmAmbience.DOFade(0.5f, 1).SetUpdate(true);
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

    public void PlayWaterSound() {
        SpookuleleAudio.AudioPlayer.PlaySound(waterSoundContainer);
    }
    public void PlayHarvestSound() { harvestSound.Play(); }
    public void PlaySearchSound() { detectedStinger.Play(); }
    public void PlayChaseSound() { chaseStinger.Play(); }

    public void PlayBackSound() {
        backSound.Play();
    }

    public void PlaySelectSound() {
        selectSound.Play();
    }
    public void PlaySubmitSound() { submitSound.Play(); }

    public void PlayFarmAmbience() {
        farmAmbience.Play();
    }
    public void PlayWalking() {
        walkingSound.Play();
    }
}
