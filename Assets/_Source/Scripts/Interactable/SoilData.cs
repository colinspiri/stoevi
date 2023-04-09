using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NewSoilData", menuName = "SoilData")]
public class SoilData : SerializedScriptableObject {
    /*private static float maxDistanceFromCenter = 1.5f;
    private static float minDistanceToOtherCrop = 1.5f;*/
    
    public List<CropData> cropData = new List<CropData>();

    public void SaveDataFromSoil(Soil soil) {
        ClearData();
        
        foreach (var crop in soil.crops) {
            Crop.GrowthStage stage = crop.stage;
        
            cropData.Add(new CropData(stage));
        }
        
        SaveToFile();
    }

    public void SaveToFile() {
        var filePath = GetFilePath();
        File.WriteAllText(filePath, "");

        var json = JsonUtility.ToJson(this);
        File.WriteAllText(filePath, json);
    }

    public void LoadDataFromFile() {
        cropData.Clear();

        var filePath = GetFilePath();

        if(!File.Exists(filePath)) {
            Debug.LogWarning($"File \"{filePath}\" not found!", this);
            return;
        }

        var json = File.ReadAllText(filePath);
        // Debug.Log("loading data from " + filePath + " : \n" + json);
        JsonUtility.FromJsonOverwrite(json, this);
    }

    public void ClearData() {
        cropData.Clear();

        var filePath = GetFilePath();
        File.WriteAllText(filePath, "");
    }

    public void AddRandomCrop(Crop.GrowthStage growthStage = Crop.GrowthStage.Sprout) {
        if (cropData.Count >= 1) return;

        cropData.Add(new CropData(growthStage));

        SaveToFile();
    }

    public bool HasSpaceInSoil() {
        return cropData.Count == 0;
    }

    private string GetFilePath() {
        return Path.Combine(Application.persistentDataPath, name + ".json");
    }
}

[Serializable]
public struct CropData {
    public Crop.GrowthStage stage;

    public CropData(Crop.GrowthStage stage) {
        this.stage = stage;
    }
}