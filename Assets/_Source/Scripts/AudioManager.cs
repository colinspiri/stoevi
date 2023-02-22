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
    public SceneLoader sceneLoader;
    
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
    public AudioSource ambience_day;
    public AudioSource ambience_night;

    [Header("SFX")] 
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
        ambience_day.ignoreListenerPause = true;
        ambience_night.ignoreListenerPause = true;

        tensionVolume = tensionMusic.volume;
        
        audioSettings.Initialize();

        // play sounds on scene start
        SceneManager.activeSceneChanged += (oldScene, newScene) => PlaySoundsOnSceneStart(newScene);
        PlaySoundsOnSceneStart(SceneManager.GetActiveScene());
    }

    private void PlaySoundsOnSceneStart(Scene newScene) {
        // reset listener pause
        AudioListener.pause = false;
        
        // stop misc sounds
        ambience_night.Stop();
        tensionMusic.Stop();
        playerBreathing.Stop();
        playerTiredBreathing.Stop();

        // main menu scene
        if (newScene.path == sceneLoader.mainMenuScene.ScenePath) {
            mainMenuMusic.Play();
        }
        else mainMenuMusic.Stop();

        // days 
        if (newScene.path == sceneLoader.day1Scene.ScenePath ||
            newScene.path == sceneLoader.day2Scene.ScenePath) {
            if(!ambience_day.isPlaying) ambience_day.Play();
        }
        else ambience_day.Stop();
    }

    public void StopIntroCutsceneMusic() {
        introCutsceneMusic.Stop();
    }

    public void PauseGameSound() {
        AudioListener.pause = true;
        ambience_day.DOFade(0.1f, 1).SetUpdate(true);
        ambience_night.DOFade(0.05f, 1).SetUpdate(true);
    }
    public void ResumeGameSound() {
        AudioListener.pause = false;
        ambience_day.DOFade(0.3f, 1).SetUpdate(true);
        ambience_night.DOFade(0.1f, 1).SetUpdate(true);
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

    public void TransitionToNightAmbience() {
        ambience_day.DOFade(0, 10f).SetUpdate(true).OnComplete(() => {
            ambience_day.Stop();
        });
        
        ambience_night.Play();
        ambience_night.volume = 0f;
        ambience_night.DOFade(0.1f, 10f).SetUpdate(true);
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
        if(rustle == null) return;
        if (value && !rustle.isPlaying) rustle.Play();
        if(rustle.isPlaying && !value) rustle.Stop();
    }

    public void PlayHarvestSound() { harvestSound.Play(); }

    public void PlayDetectedStinger() {
        detectedStinger.Play();
        torbalanInhale.Play();
    }
    public void PlayChaseStinger() { chaseStinger.Play(); }

    public void PlayBackSound() { backSound.Play(); }
    public void PlaySelectSound() { selectSound.Play(); }
    public void PlaySubmitSound() { submitSound.Play(); }

    public void PlayDayAmbience() { ambience_day.Play(); }
    public void PlayWalking() { walkingSound.Play(); }
}
