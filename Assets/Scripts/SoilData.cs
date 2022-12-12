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

    public void SaveCropData(Crop crop) {
        Vector3 position = crop.transform.position;
        Crop.GrowthStage stage = crop.stage;
        
        cropData.Add(new CropData(position, stage));
    }

    public void SaveToFile() {
        var filePath = Path.Combine(Application.persistentDataPath, name + ".json");

        var json = JsonUtility.ToJson(this);
        Debug.Log("saving data to " + filePath + " : \n" + json);
        File.WriteAllText(filePath, json);
    }
    
    public void LoadDataFromFile() {
        ClearData();

        var filePath = Path.Combine(Application.persistentDataPath, name + ".json");

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
    }
}

public struct CropData {
    public Vector3 position;
    public Crop.GrowthStage stage;

    public CropData(Vector3 position, Crop.GrowthStage stage) {
        this.position = position;
        this.stage = stage;
    }
}