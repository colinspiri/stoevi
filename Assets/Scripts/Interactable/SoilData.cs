using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityParticleSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "NewSoilData", menuName = "SoilData")]
public class SoilData : SerializedScriptableObject {
    private static float maxDistanceFromCenter = 1.5f;
    
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

    private void SaveToFile() {
        var filePath = GetFilePath();
        File.WriteAllText(filePath, "");

        var json = JsonUtility.ToJson(this);
        // Debug.Log("saving data to " + filePath + " : \n" + json);
        File.WriteAllText(filePath, json);
    }

    public void OnEditorRefresh() {
        SaveToFile();
    }
    
    public void LoadDataFromFile() {
        cropData.Clear();

        var filePath = GetFilePath();

        if(!File.Exists(filePath)) {
            Debug.LogWarning($"File \"{filePath}\" not found!", this);
            return;
        }

        if (PlayerPrefs.GetInt("CurrentDay", 1) == 1) {
            File.WriteAllText(filePath, "");
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

    public void AddRandomCrop() {
        if (cropData.Count >= 2) return;

        Vector3 randomPosition = Vector3.zero;
        randomPosition.x = Random.Range(-maxDistanceFromCenter, maxDistanceFromCenter);
        randomPosition.z = Random.Range(-maxDistanceFromCenter, maxDistanceFromCenter);
        
        cropData.Add(new CropData(randomPosition, Crop.GrowthStage.Seed));
    }

    private string GetFilePath() {
        return Path.Combine(Application.persistentDataPath, name + ".json");
    }
}

public struct CropData {
    public Vector3 relativePosition;
    public Crop.GrowthStage stage;

    public CropData(Vector3 relativePosition, Crop.GrowthStage stage) {
        this.relativePosition = relativePosition;
        this.stage = stage;
    }
}