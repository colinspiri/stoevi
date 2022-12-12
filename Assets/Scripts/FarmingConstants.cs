using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;

[CreateAssetMenu(fileName = "FarmingConstants", menuName = "FarmingConstants")]
public class FarmingConstants : ScriptableObject {
    
    [Header("Components")]
    public Sprite waterSprite;
    public Sprite harvestSprite;
    public Sprite emptySprite;

    [Header("Soil")] 
    public int maxFertilizerLevel;
    public int maxCrops;

    [Header("Crops")] 
    public float baseThirstyTime;
    public SerializedDictionary<Crop.GrowthStage, float> growthTimeByStage;
    public List<float> consecutiveTimerMultipliers;
}