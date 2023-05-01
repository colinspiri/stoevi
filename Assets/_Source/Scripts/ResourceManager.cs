using UnityEngine;

public class ResourceManager : MonoBehaviour {
    public static ResourceManager Instance;
    
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
        Instance = this;
        
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
            
            PlayerPrefs.DeleteKey("Seeds");
            PlayerPrefs.DeleteKey("Fertilizer");
            PlayerPrefs.DeleteKey("Batteries");
            PlayerPrefs.DeleteKey("BatteryPercent");
            PlayerPrefs.DeleteKey("CurrentBalance");
        }
        else {
            seeds.SetValue(PlayerPrefs.GetInt("Seeds", seeds.Value));
            currentFertilizer.SetValue(PlayerPrefs.GetInt("Fertilizer", currentFertilizer.Value));
            currentBatteries.SetValue(PlayerPrefs.GetInt("Batteries", currentBatteries.Value));
            batteryPercent.SetValue(PlayerPrefs.GetFloat("BatteryPercent", batteryPercent.Value));
        }
    }

    public void SaveAllData() {
        PlayerPrefs.SetInt("Seeds", seeds.Value);
        PlayerPrefs.SetInt("Fertilizer", currentFertilizer.Value);
        PlayerPrefs.SetInt("Batteries", currentBatteries.Value);
        PlayerPrefs.SetFloat("BatteryPercent", batteryPercent.Value);
    }
}
