using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SpookuleleAudio;
using StarterAssets;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;
    public AudioSettings audioSettings;

    [Header("Scenes")]
    public SceneReference mainMenuScene;
    public SceneReference introCutscene;
    public SceneReference gameScene;
    
    [Header("Music")]
    public AudioSource mainMenuMusic;
    public AudioSource introCutsceneMusic;
    public ASoundContainer detectedStinger;
    public ASoundContainer chaseStinger;
    [Space]
    public AudioSource tensionMusic;
    public float tensionFadeTime;
    public float tensionFadeInDistance;
    public float tensionFadeOutDistance;
    private float tensionVolume;
    private bool tensionFading;

    [Header("Ambience")] 
    public AudioSource farmAmbience;

    [Header("SFX")] 
    public ASoundContainer waterSound;
    public AudioSource walkingSound;
    public AudioSource runningSound;
    public ASoundContainer harvestSound;
    public AudioSource playerBreathing;
    public AudioSource playerTiredBreathing;
    public ASoundContainer torbalanInhale;
    public AudioSource rustle;

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
        mainMenuMusic.ignoreListenerPause = true;
        farmAmbience.ignoreListenerPause = true;

        tensionVolume = tensionMusic.volume;
        
        audioSettings.Initialize();

        // play sounds on scene start
        SceneManager.activeSceneChanged += (oldScene, newScene) => PlaySoundsOnSceneStart(newScene);
        PlaySoundsOnSceneStart(SceneManager.GetActiveScene());
    }

    private void PlaySoundsOnSceneStart(Scene newScene) {
        // reset listener pause
        AudioListener.pause = false;

        // main menu scene
        if (newScene.path == mainMenuScene.ScenePath) {
            mainMenuMusic.Play();
        }
        else mainMenuMusic.Stop();

        // game scene
        if (newScene.path == gameScene.ScenePath) {
            if (!farmAmbience.isPlaying) farmAmbience.Play();
        }
        else farmAmbience.Stop();
    }

    public void PauseGameSound() {
        AudioListener.pause = true;
        farmAmbience.DOFade(0.1f, 1).SetUpdate(true);
    }
    public void ResumeGameSound() {
        AudioListener.pause = false;
        farmAmbience.DOFade(0.3f, 1).SetUpdate(true);
    }

    private void Update() {
        // walking & running
        if (FirstPersonMovement.Instance) {
            var state = FirstPersonMovement.Instance.moveState;
            if (state == FirstPersonMovement.MoveState.Still || state == FirstPersonMovement.MoveState.CrouchWalking) {
                walkingSound.Stop();
                runningSound.Stop();
            }
            else if (state == FirstPersonMovement.MoveState.Walking) {
                if (!walkingSound.isPlaying) walkingSound.Play();
                runningSound.Stop();
            }
            else if (state == FirstPersonMovement.MoveState.Running) {
                walkingSound.Stop();
                if(!runningSound.isPlaying) runningSound.Play();
            }
        }
        
        // tension music
        if (FirstPersonMovement.Instance && TorbalanDirector.Instance) {
            float distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position,
                TorbalanDirector.Instance.transform.position);

            if (distance < tensionFadeInDistance && !tensionMusic.isPlaying && !tensionFading) {
                tensionMusic.Play();
                tensionMusic.volume = 0;
                tensionMusic.DOFade(tensionVolume, tensionFadeTime).OnComplete(() => {
                    tensionFading = false;
                });
                tensionFading = true;
            }

            if (distance > tensionFadeOutDistance && tensionMusic.isPlaying && !tensionFading) {
                tensionFading = true;
                tensionMusic.DOFade(0, tensionFadeTime).OnComplete(() => {
                    tensionMusic.Stop();
                    tensionFading = false;
                });
            }
        }
    }

    public void PlayIntroCutsceneMusic() {
        introCutsceneMusic.Play();
    }

    public void SetBreathingSound(bool value) {
        if (value && !playerBreathing.isPlaying) playerBreathing.Play();
        if(playerBreathing.isPlaying && !value) playerBreathing.Stop();
    }
    public void SetTiredBreathingSound(bool value) {
        if (value && !playerTiredBreathing.isPlaying) playerTiredBreathing.Play();
        if(playerTiredBreathing.isPlaying && !value) playerTiredBreathing.Stop();
    }
    public void SetRustle(bool value) {
        if (value && !rustle.isPlaying) rustle.Play();
        if(rustle.isPlaying && !value) rustle.Stop();
    }

    public void PlayWaterSound() { waterSound.Play(); }
    public void PlayHarvestSound() { harvestSound.Play(); }

    public void PlayDetectedStinger() {
        detectedStinger.Play();
        torbalanInhale.Play();
    }
    public void PlayChaseStinger() { chaseStinger.Play(); }

    public void PlayBackSound() { backSound.Play(); }
    public void PlaySelectSound() { selectSound.Play(); }
    public void PlaySubmitSound() { submitSound.Play(); }

    public void PlayFarmAmbience() { farmAmbience.Play(); }
    public void PlayWalking() { walkingSound.Play(); }
}
