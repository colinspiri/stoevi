using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;

[CreateAssetMenu(fileName = "FarmingConstants", menuName = "FarmingConstants")]
public class FarmingConstants : ScriptableObject {

    [Header("Materials")] 
    public Material sproutHealthyWater;
    public Material sproutHealthyGrowing;
    public Material sproutWiltedWater;
    public Material sproutWiltedGrowing;
    [Space]
    public Material intermediateHealthyWater;
    public Material intermediateHealthyGrowing;
    public Material intermediateWiltedWater;
    public Material intermediateWiltedGrowing;
    public Material intermediateDead;
    [Space] 
    public Material unripeHealthyWater;
    public Material unripeHealthyGrowing;
    public Material unripeWiltedWater;
    public Material unripeWiltedGrowing;
    public Material unripeDead;
    [Space] 
    public Material ripeHealthy;
    public Material ripeWilted;
    [Space] 
    public Material bareHealthy;
    public Material bareWilted;

    [Header("Soil")] 
    public int maxCrops;

    [Header("Crops")] 
    public float baseThirstyTime;
    public float baseGrowthTime;
    public float fertilizedThirstyTime;
    public float fertilizedGrowthTime;
}