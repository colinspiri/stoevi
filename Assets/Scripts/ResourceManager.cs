using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour {
    public static ResourceManager Instance;
    
    // water
    public int maxWater; // 4
    private int currentWater;
    public UnityEvent<float> onWaterValueChange;
    
    // tomatoes
    public int PlayerTomatoes { get; private set; }
    public int TorbalanTomatoes { get; private set; }
    public UnityEvent<int> playerHarvestedTomato;
    
    // seeds
    public int maxSeeds;
    public int CurrentSeeds { get; private set; }
    public UnityEvent<int> onSeedsChange;

    
    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        currentWater = maxWater;
        CurrentSeeds = maxSeeds;
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
        PlayerTomatoes++;
        playerHarvestedTomato?.Invoke(PlayerTomatoes);
    }
    public void TorbalanStoleTomato() {
        TorbalanTomatoes++;
    }

    public void UseSeed() {
        CurrentSeeds--;
        onSeedsChange?.Invoke(CurrentSeeds);
    }
    public bool HasSeedsLeft() {
        return CurrentSeeds > 0;
    }
}
