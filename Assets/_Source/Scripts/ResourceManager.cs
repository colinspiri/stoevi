using UnityEngine;

public class ResourceManager : MonoBehaviour {
    // water
    [Header("Water")]
    public IntVariable maxWater;
    public IntVariable currentWater;
    
    // tomatoes
    [Header("Tomatoes")]
    public IntVariable playerTomatoes;
    public IntVariable torbalanTomatoes;
    
    // seeds
    [Header("Seeds")]
    public IntReference startingSeeds;
    public IntVariable seeds;

    [Header("Fertilizer")] 
    public IntReference startingFertilizer;
    public IntVariable currentFertilizer;

    [Header("Flashlight")] 
    public IntReference startingBatteries;
    public IntVariable currentBatteries;
    public FloatVariable batteryPercent;

    void Awake() {
        currentWater.SetValue(maxWater.Value);
        
        playerTomatoes.SetValue(0);
        torbalanTomatoes.SetValue(0);

        // set to starting values
        int day = PlayerPrefs.GetInt("CurrentDay", 1);
        if (day == 1) {
            seeds.SetValue(startingSeeds.Value);
            currentFertilizer.SetValue(startingFertilizer.Value);
            currentBatteries.SetValue(startingBatteries.Value);
            batteryPercent.SetValue(1);
        }
    }
}
