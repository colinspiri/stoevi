using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    
    // day
    public int currentDay;

    // game state
    public bool gameStopped;

    private void Awake() {
        Instance = this;
        currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
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
    
    public void GameOver(bool playerSurvived = true) {
        Pause(true);
        currentDay++;
        PlayerPrefs.SetInt("CurrentDay", currentDay);
        PauseMenuManager.Instance.GameOver(playerSurvived);
    }

    public void OnDebugGameOverInput(InputAction.CallbackContext context) {
        if (context.performed) {
            GameOver(true);
        }
    }
}
