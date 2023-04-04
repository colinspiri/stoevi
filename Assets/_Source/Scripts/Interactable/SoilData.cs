using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NewSoilData", menuName = "SoilData")]
public class SoilData : SerializedScriptableObject {
    private static float maxDistanceFromCenter = 1.5f;
    private static float minDistanceToOtherCrop = 1.5f;
    
    public List<CropData> cropData = new List<CropData>();

    public void SaveDataFromSoil(Soil soil) {
        ClearData();
        
        foreach (var crop in soil.crops) {
            Vector3 relativePosition = crop.transform.localPosition;
            relativePosition.y = 0;
            Crop.GrowthStage stage = crop.stage;
        
            cropData.Add(new CropData(relativePosition, stage));
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
        if (cropData.Count >= 2) return;

        Vector3 randomPosition = Vector3.zero;
        int maxLoops = 1000;
        for (int loops = 0; loops <= maxLoops; loops++) {
            randomPosition.x = Random.Range(-maxDistanceFromCenter, maxDistanceFromCenter);
            randomPosition.z = Random.Range(-maxDistanceFromCenter, maxDistanceFromCenter);

            // if no other crops, accept random position
            if (cropData.Count == 0) break;
            
            // if there is other crop, calculate distance
            bool passesDistanceCriteria = true;
            foreach (var data in cropData) {
                float distance = Vector3.Distance(randomPosition, data.relativePosition);
                if (distance == 0) continue;
                if (distance <= minDistanceToOtherCrop) {
                    passesDistanceCriteria = false;
                    break;
                }
            }
            if (passesDistanceCriteria) break;
            // reset random position for next loop
            randomPosition = Vector3.zero;
        }
        
        cropData.Add(new CropData(randomPosition, growthStage));

        SaveToFile();
    }

    public bool HasSpaceInSoil() {
        return cropData.Count < 2;
    }

    private string GetFilePath() {
        return Path.Combine(Application.persistentDataPath, name + ".json");
    }
}

[Serializable]
public struct CropData {
    public Vector3 relativePosition;
    public Crop.GrowthStage stage;

    public CropData(Vector3 relativePosition, Crop.GrowthStage stage) {
        this.relativePosition = relativePosition;
        this.stage = stage;
    }
}