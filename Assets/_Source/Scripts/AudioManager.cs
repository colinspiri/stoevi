using System.Reflection;
using DG.Tweening;
using SpookuleleAudio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;
    public AudioSettings audioSettings;

    [Header("Scenes")] 
    public SceneLoader sceneLoader;
    
    [Header("Music")]
    public AudioSource mainMenuMusic;
    public AudioSource themeBassMusic;
    private float themeBassMusicVolume;
    public ASoundContainer detectedStinger;
    public ASoundContainer chaseStinger;
    [Space]
    public AudioSource tensionDay23;
    public AudioSource tensionDay45;
    public float tensionFadeTime;
    public float tensionFadeInDistance;
    public float tensionFadeOutDistance;
    private float tensionVolume;
    private bool tensionFading;
    [Space] 
    public AudioSource chaseDay23;
    public AudioSource chaseDay45;
    public float chaseFadeTime;
    private float chaseVolume;
    private bool chaseFading;

    [Header("Ambience")] 
    public AudioSource ambience_day;
    public AudioSource ambience_night;
    private float ambienceDayVolume;
    private float ambienceNightVolume;

    [Header("SFX")] private bool rustle;
    public AudioSource walkingSound;
    public AudioSource runningSound;
    public AudioSource rustleWalking;
    public AudioSource rustleRunning;
    public AudioSource playerBreathing;
    public AudioSource playerTiredBreathing;
    public ASoundContainer torbalanInhale;

    [Header("UI")]
    public ASoundContainer backSound;
    public ASoundContainer selectSound;
    public ASoundContainer submitSound;
    
    // state
    private int day;

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

        themeBassMusicVolume = themeBassMusic.volume;
        tensionVolume = tensionDay23.volume;
        chaseVolume = chaseDay23.volume;
        ambienceDayVolume = ambience_day.volume;
        ambienceNightVolume = ambience_night.volume;
        Debug.Log("day = " + ambienceDayVolume + " night = " + ambienceNightVolume);
        
        audioSettings.Initialize();

        // play sounds on scene start
        SceneManager.activeSceneChanged += (oldScene, newScene) => {
            Debug.Log("active scene changed from " + oldScene.name + " to " + newScene.name);
            PlaySoundsOnSceneStart(oldScene, newScene);
        };
        PlaySoundsOnSceneStart(new Scene(), SceneManager.GetActiveScene());
    }

    private void PlaySoundsOnSceneStart(Scene oldScene, Scene newScene) {
        // reset listener pause
        AudioListener.pause = false;
        
        // stop misc sounds
        ambience_night.Stop();
        tensionDay23.Stop();
        tensionDay45.Stop();
        chaseDay23.Stop();
        chaseDay45.Stop();
        playerBreathing.Stop();
        playerTiredBreathing.Stop();
        
        
        // theme bass music
        if (newScene.path == sceneLoader.cutsceneScene.ScenePath) {
            themeBassMusic.Play();
            themeBassMusic.volume = themeBassMusicVolume;
        }
        else if (themeBassMusic.isPlaying) {
            themeBassMusic.DOFade(0, 5f).SetUpdate(true).OnComplete(() => {
                themeBassMusic.Stop();
            });
        }

        // main menu scene
        if (newScene.path == sceneLoader.mainMenuScene.ScenePath) {
            mainMenuMusic.Play();
        }
        else mainMenuMusic.Stop();

        
        bool isDay = false;
        foreach (var dayScene in sceneLoader.dayScenes) {
            if (newScene.path == dayScene.ScenePath) {
                isDay = true;
                break;
            }
        }
        // ambience 
        if (isDay || newScene.path == sceneLoader.cutsceneScene.ScenePath) {
            if(!ambience_day.isPlaying) ambience_day.Play();
        }
        else ambience_day.Stop();
        
        // tension
        day = PlayerPrefs.GetInt("CurrentDay", 1);
        if (isDay) {
            Debug.Log("start playing tension music");
            AudioSource tension = day >= 4 ? tensionDay45 : tensionDay23;
            AudioSource chase = day >= 4 ? chaseDay45 : chaseDay23;
            tension.Play();
            tension.volume = 0;
            chase.Play();
            chase.volume = 0;
        }
    }

    public void StopIntroCutsceneMusic() {
        themeBassMusic.Stop();
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
                rustleWalking.Stop();
                rustleRunning.Stop();
            }
            else if (state == FirstPersonMovement.MoveState.Walking) {
                if (rustle) {
                    if (!rustleWalking.isPlaying) rustleWalking.Play();
                    walkingSound.Stop();
                }
                else {
                    if(!walkingSound.isPlaying) walkingSound.Play();
                    rustleWalking.Stop();
                }
                
                runningSound.Stop();
                rustleRunning.Stop();
            }
            else if (state == FirstPersonMovement.MoveState.Running) {
                if (rustle) {
                    if(!rustleRunning.isPlaying) rustleRunning.Play();
                    runningSound.Stop();
                }
                else {
                    if(!runningSound.isPlaying) runningSound.Play();
                    rustleRunning.Stop();
                }

                walkingSound.Stop();
                rustleWalking.Stop();
            }
        }
        
        // torbalan music
        if (FirstPersonMovement.Instance && TorbalanDirector.Instance) {
            // tension music
            float distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position,
                TorbalanDirector.Instance.transform.position);

            AudioSource tension = day >= 4 ? tensionDay45 : tensionDay23;
            
            if (distance < tensionFadeInDistance && tension.volume == 0 && !tensionFading) {
                tension.DOFade(tensionVolume, tensionFadeTime).OnComplete(() => {
                    tensionFading = false;
                });
                tensionFading = true;
            }

            if (distance > tensionFadeOutDistance && tension.volume == tensionVolume && !tensionFading) {
                tension.DOFade(0, tensionFadeTime).OnComplete(() => {
                    tensionFading = false;
                });
                tensionFading = true;
            }
            
            // chase music
            TorbalanStateTracker.TorbalanState state = TorbalanStateTracker.Instance.currentState;
            AudioSource chase = day >= 4 ? chaseDay45 : chaseDay23;

            if (state == TorbalanStateTracker.TorbalanState.Chase && chase.volume == 0 && !chaseFading) {
                chase.DOFade(chaseVolume, chaseFadeTime).OnComplete(() => {
                    chaseFading = false;
                });
                chaseFading = true;
            }

            if (state != TorbalanStateTracker.TorbalanState.Chase && chase.volume == chaseVolume && !chaseFading) {
                chase.DOFade(0, chaseFadeTime).OnComplete(() => {
                    chaseFading = false;
                });
                chaseFading = true;
            }
        }
    }

    public void PlayDayAmbience(float fadeInDuration = 1f) {
        ambience_day.Play();
        ambience_day.volume = 0;
        ambience_day.DOFade(ambienceDayVolume, fadeInDuration);
    }
    public void TransitionToNightAmbience() {
        ambience_day.DOFade(0, 20f).SetUpdate(true).OnComplete(() => {
            ambience_day.Stop();
        });
        
        ambience_night.Play();
        ambience_night.volume = 0f;
        ambience_night.DOFade(ambienceNightVolume, 20f).SetUpdate(true);
    }

    public void PlayIntroCutsceneMusic() {
        themeBassMusic.Play();
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
        rustle = value;
    }
    
    public void PlayDetectedStinger() {
        detectedStinger.Play();
        torbalanInhale.Play();
    }
    public void PlayChaseStinger() { chaseStinger.Play(); }

    public void PlayBackSound() { backSound.Play(); }
    public void PlaySelectSound() { selectSound.Play(); }
    public void PlaySubmitSound() { submitSound.Play(); }
}
