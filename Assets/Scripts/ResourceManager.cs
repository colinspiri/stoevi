using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour {
    public static ResourceManager Instance;
    
    // water
    public IntVariable maxWater; // 5
    public IntVariable currentWater;
    
    // tomatoes
    public IntVariable playerTomatoes;
    public IntVariable torbalanTomatoes;
    public UnityEvent<int> playerHarvestedTomato;
    
    // seeds
    public IntReference startingSeeds;
    public IntVariable seeds;
    
    // fertilizer
    public bool carryingFertilizer { get; private set; }
    public GameObject fertilizerPrefab;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        currentWater.SetValue(maxWater.Value);
        seeds.SetValue(startingSeeds.Value);
    }

    public void UseWater() {
        currentWater.ApplyChange(-1);
    }
    public void RefillWater() {
        currentWater.SetValue(maxWater);
    }

    public void PlayerHarvestedTomato() {
        playerTomatoes.ApplyChange(1);
        playerHarvestedTomato?.Invoke(playerTomatoes.Value);
    }
    public void TorbalanStoleTomato() {
        torbalanTomatoes.ApplyChange(1);
    }

    public void PickUpFertilizer() {
        carryingFertilizer = true;
    }
    public void DropFertilizer() {
        carryingFertilizer = false;
        Instantiate(fertilizerPrefab, FirstPersonMovement.Instance.transform.position + 2*FirstPersonMovement.Instance.transform.forward, Quaternion.identity);
    }
}
