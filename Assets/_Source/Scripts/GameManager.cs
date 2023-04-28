using System;
using SpookuleleAudio;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    public SceneLoader sceneLoader;
    
    // day
    public int currentDay;

    // game state
    public bool gameStopped;
    
    public static event Action<bool> OnGameOver = delegate {  };

    private void Awake() {
        Instance = this;
        
        currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        if (currentDay == 1) {
            PlayerPrefs.DeleteAll();
        }
    }
    
    public void Pause(bool pauseAudio = false) {
        gameStopped = true;
        Time.timeScale = 0.0f;

        if (pauseAudio) {
            if (AudioManager.Instance) AudioManager.Instance.PauseGameSound();
        }
    }

    public void Resume(bool resumeAudio = false) {
        gameStopped = false;
        Time.timeScale = 1.0f;

        if (resumeAudio) {
            if (AudioManager.Instance) AudioManager.Instance.ResumeGameSound();
            else Debug.LogError("Audio Manager not found");
        }
    }

    public void EndDay() {
        if (gameStopped) return;
        
        currentDay++;
        PlayerPrefs.SetInt("CurrentDay", currentDay);
            
        InteractableManager.Instance.SaveAllData();

        if (currentDay == 6) {
            sceneLoader.LoadCutscene();
        }
        else sceneLoader.LoadShop();
    }
    
    public void GameOver(bool playerSurvived = true) {
        if (gameStopped) return;
        
        Pause(true);

        OnGameOver(playerSurvived);
    }
}
