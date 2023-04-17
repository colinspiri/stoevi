using UnityEngine;

[CreateAssetMenu(fileName = "FarmingConstants", menuName = "FarmingConstants")]
public class FarmingConstants : ScriptableObject {

    [Header("Materials")] 
    public Material sproutHealthyWater;
    public Material sproutHealthyGrowing;
    public Material sproutWiltedWater;
    public Material sproutWiltedGrowing;
    public Material sproutDead;
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
    public Material ripeDead;
    [Space] 
    public Material bareHealthy;
    public Material bareWilted;
    public Material bareDead;

    [Header("Soil")] 
    public int maxCrops;

    [Header("Crops")] 
    public float baseGrowthTime;
    public float fertilizedGrowthTime;
}