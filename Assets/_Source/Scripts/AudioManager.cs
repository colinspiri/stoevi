using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using SpookuleleAudio;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Yarn.Unity;

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
    [Space] 
    public AudioSource themeStingerSource;
    public List<AudioClip> themeStingers;


    [Header("Ambience")] 
    public AudioSource ambience_day;
    public AudioSource ambience_day_rain;
    public AudioSource ambience_night;
    public AudioSource ambience_night_rain;
    private float ambienceDayVolume;
    private float ambienceDayRainVolume;
    private float ambienceNightVolume;
    private float ambienceNightRainVolume;

    [Header("SFX")] private bool rustle;
    public AudioSource walkingSound;
    public AudioSource runningSound;
    public AudioSource rustleWalking;
    public AudioSource rustleRunning;
    public AudioSource playerBreathing;
    public AudioSource playerTiredBreathing;
    public ASoundContainer torbalanInhale;
    public ASoundContainer sheep_bleat;
    public ASoundContainer sheep_hit;

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
        ambience_day_rain.ignoreListenerPause = true;
        ambience_night.ignoreListenerPause = true;
        ambience_night_rain.ignoreListenerPause = true;

        themeBassMusicVolume = themeBassMusic.volume;
        tensionVolume = tensionDay23.volume;
        chaseVolume = chaseDay23.volume;
        ambienceDayVolume = ambience_day.volume;
        ambienceDayRainVolume = ambience_day_rain.volume;
        ambienceNightVolume = ambience_night.volume;
        ambienceNightRainVolume = ambience_night_rain.volume;
        
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
        ambience_night_rain.Stop();
        tensionDay23.Stop();
        tensionDay45.Stop();
        chaseDay23.Stop();
        chaseDay45.Stop();
        playerBreathing.Stop();
        playerTiredBreathing.Stop();

        // theme bass music
        if (newScene.path == sceneLoader.cutsceneScene.ScenePath || newScene.path == sceneLoader.endCreditsScene.ScenePath) {
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

        
        day = PlayerPrefs.GetInt("CurrentDay", 1);
        bool isDay = false;
        foreach (var dayScene in sceneLoader.dayScenes) {
            if (newScene.path == dayScene.ScenePath) {
                isDay = true;
                break;
            }
        }
        // ambience 
        if (day >= 4) {
            if (isDay || newScene.path == sceneLoader.cutsceneScene.ScenePath || newScene.path == sceneLoader.endCreditsScene.ScenePath || newScene.path == sceneLoader.shopScene.ScenePath) {
                if(!ambience_day_rain.isPlaying) ambience_day_rain.Play();
            }
            else ambience_day_rain.Stop();
            ambience_day.Stop();
        }
        else {
            if (isDay || newScene.path == sceneLoader.cutsceneScene.ScenePath || newScene.path == sceneLoader.endCreditsScene.ScenePath || newScene.path == sceneLoader.shopScene.ScenePath) {
                if(!ambience_day.isPlaying) ambience_day.Play();
            }
            else ambience_day.Stop();
            ambience_day_rain.Stop();
        }
        
        
        // tension
        if (isDay) {
            AudioSource tension = day >= 4 ? tensionDay45 : tensionDay23;
            AudioSource chase = day >= 4 ? chaseDay45 : chaseDay23;
            tension.Play();
            tension.volume = 0;
            chase.Play();
            chase.volume = 0;
        }
    }
    
    [YarnCommand("theme")]
    public static void PlayTheme(int number) {
        int index = number - 1;
        if (index < 0 || index >= Instance.themeStingers.Count) {
            Debug.Log("theme " + number + " is out of range");
        }
        
        Debug.Log("Playing theme stinger " + number);

        AudioClip clip = Instance.themeStingers[index];
        Instance.themeStingerSource.clip = clip;
        Instance.themeStingerSource.Play();
    }

    public void StopIntroCutsceneMusic() {
        themeBassMusic.Stop();
    }

    public void PauseGameSound() {
        AudioListener.pause = true;
        ambience_day.DOFade(0.1f, 1).SetUpdate(true);
        ambience_day_rain.DOFade(0.1f, 1).SetUpdate(true);
        ambience_night.DOFade(0.05f, 1).SetUpdate(true);
        ambience_night_rain.DOFade(0.05f, 1).SetUpdate(true);
    }
    public void ResumeGameSound() {
        AudioListener.pause = false;
        ambience_day.DOFade(ambienceDayVolume, 1).SetUpdate(true);
        ambience_day_rain.DOFade(ambienceDayRainVolume, 1).SetUpdate(true);
        ambience_night.DOFade(ambienceNightVolume, 1).SetUpdate(true);
        ambience_night_rain.DOFade(ambienceNightRainVolume, 1).SetUpdate(true);
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
        AudioSource current_ambience_day = day >= 4 ? ambience_day_rain : ambience_day;
        float current_ambience_volume = day >= 4 ? ambienceDayRainVolume : ambienceDayVolume;
        
        current_ambience_day.Play();
        current_ambience_day.volume = 0;
        current_ambience_day.DOFade(current_ambience_volume, fadeInDuration);
    }
    public void TransitionToNightAmbience() {
        AudioSource current_ambience_day = day >= 4 ? ambience_day_rain : ambience_day;
        
        current_ambience_day.DOFade(0, 20f).SetUpdate(true).OnComplete(() => {
            current_ambience_day.Stop();
        });
        
        AudioSource current_ambience_night = day >= 4 ? ambience_night_rain : ambience_night;
        float current_ambience_volume = day >= 4 ? ambienceNightRainVolume : ambienceNightVolume;
        
        current_ambience_night.Play();
        current_ambience_night.volume = 0f;
        current_ambience_night.DOFade(current_ambience_volume, 20f).SetUpdate(true);
    }

    public void PlayThemeBassMusic() {
        if(themeBassMusic.isPlaying == false) themeBassMusic.Play();
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

    [YarnCommand("sheep_bleat")]
    public static void PlayBleat() {
        Instance.sheep_bleat.Play();
    }
    [YarnCommand("sheep_hit")]
    public static void PlaySheepHit() {
        Instance.sheep_hit.Play();
    }
}
