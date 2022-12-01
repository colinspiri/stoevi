using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    
    // water
    public int maxWater; // 4
    private int currentWater;
    public UnityEvent<float> onWaterValueChange;
    
    // tomatoes
    private int playerTomatoes;
    public int PlayerTomatoes => playerTomatoes;
    private int torbalanTomatoes;
    public int TorbalanTomatoes => torbalanTomatoes;
    public UnityEvent<int> playerHarvestedTomato;
    
    // game state
    public bool gameStopped;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        currentWater = maxWater;
    }
    
    public void UseWater() {
        currentWater--;
        onWaterValueChange?.Invoke((float)currentWater/maxWater);
    }
    public void RefillWater() {
        currentWater = maxWater;
        onWaterValueChange?.Invoke((float)currentWater/maxWater);
    }
    public bool IsWaterEmpty() {
        return currentWater <= 0;
    }
    public bool IsWaterFull() {
        return currentWater == maxWater;
    }

    public void PlayerHarvestedTomato() {
        playerTomatoes++;
        playerHarvestedTomato?.Invoke(playerTomatoes);
    }

    public void TorbalanStoleTomato() {
        torbalanTomatoes++;
    }

    public void Pause(bool pauseAudio = false) {
        gameStopped = true;
        Time.timeScale = 0.0f;

        if (pauseAudio) {
            if (AudioManager.Instance) AudioManager.Instance.PauseGameSound();
            else Debug.LogError("Audio Manager not found");
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
        PauseMenuManager.Instance.GameOver(playerSurvived);
    }
}
