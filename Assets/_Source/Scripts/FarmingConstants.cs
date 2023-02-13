using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;

[CreateAssetMenu(fileName = "FarmingConstants", menuName = "FarmingConstants")]
public class FarmingConstants : ScriptableObject {
    
    [Header("Components")]
    public Material waterMaterial;
    public Material harvestMaterial;
    public Material emptyMaterial;

    [Header("Soil")] 
    public int maxCrops;

    [Header("Crops")] 
    public float baseThirstyTime;
    public float baseGrowthTime;
    public float fertilizedThirstyTime;
    public float fertilizedGrowthTime;
}