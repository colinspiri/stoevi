using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CropSpawner : MonoBehaviour {
    // constants
    public SerializedDictionary<Crop.GrowthStage, int> cropsToSpawnByStage;

    // components
    public List<SoilData> allSoilData;

    void Awake() {
        int day = PlayerPrefs.GetInt("CurrentDay", 1);
        if (day == 1) {
            // Debug.Log("CropSpawner spawning crops on day 1");
            ClearAllSoilData();
            SpawnCrops();
        }
    }

    private void ClearAllSoilData() {
        foreach (var soilData in allSoilData) {
            soilData.ClearData();
        }
    }

    private void SpawnCrops() {
        foreach (var pair in cropsToSpawnByStage) {
            for (int i = 0; i < pair.Value; i++) {
                SpawnCropOnRandomSoil(pair.Key);
            }
        }
    }

    private void SpawnCropOnRandomSoil(Crop.GrowthStage growthStage) {
        // select random soil with space
        SoilData randomSoil = null;
        do {
            int randomIndex = Random.Range(0, allSoilData.Count);
            randomSoil = allSoilData[randomIndex];
        } while (!randomSoil.HasSpaceInSoil());
            
        // spawn crop on soil
        randomSoil.AddRandomCrop(growthStage);
        randomSoil.SaveToFile();
    }

}
