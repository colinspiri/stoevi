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
    
    public void GameOver(bool playerSurvived = true) {
        PauseMenuManager.Instance.GameOver(playerSurvived);
    }
}
