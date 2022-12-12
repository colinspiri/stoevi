using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

[CreateAssetMenu(fileName = "FarmingConstants", menuName = "FarmingConstants")]
public class FarmingConstants : ScriptableObject {
    
    [Header("Components")]
    public Sprite waterSprite;
    public Sprite harvestSprite;
    public Sprite emptySprite;

    [Header("Crops")] 
    public SerializedDictionary<Crop.CropStage, float> growthTimeByStage;
    public List<float> growthTimeConsecutivePenalties;

}