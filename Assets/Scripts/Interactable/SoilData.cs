using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityParticleSystem;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSoilData", menuName = "SoilData")]
public class SoilData : SerializedScriptableObject {
    public List<CropData> cropData = new List<CropData>();

    public void SaveDataFromSoil(Soil soil) {
        ClearData();
        
        foreach (var crop in soil.crops) {
            Vector3 relativePosition = crop.transform.position;
            Crop.GrowthStage stage = crop.stage;
        
            cropData.Add(new CropData(relativePosition, stage));
        }
        
        SaveToFile();
    }

    private void SaveToFile() {
        var filePath = GetFilePath();

        var json = JsonUtility.ToJson(this);
        Debug.Log("saving data to " + filePath + " : \n" + json);
        File.WriteAllText(filePath, json);
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
        Debug.Log("loading data from " + filePath + " : \n" + json);
        JsonUtility.FromJsonOverwrite(json, this);
    }

    public void ClearData() {
        cropData.Clear();

        var filePath = GetFilePath();
        File.WriteAllText(filePath, "");
        
        Debug.Log("data on " + name + " cleared");
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