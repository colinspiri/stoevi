using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour {
    public static ResourceManager Instance;
    
    // water
    public IntVariable maxWater;
    public IntVariable currentWater;
    
    // tomatoes
    public IntVariable playerTomatoes;
    public IntVariable torbalanTomatoes;
    
    // seeds
    public IntReference startingSeeds;
    public IntVariable seeds;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        currentWater.SetValue(maxWater.Value);
        seeds.SetValue(startingSeeds.Value);
        playerTomatoes.SetValue(0);
        torbalanTomatoes.SetValue(0);
    }
}
