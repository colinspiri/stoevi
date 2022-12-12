using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Compiler;

public class Soil : Interactable {
    // components
    public GameObject seedPrefab;
    
    // constants
    public int maxCrops;
    public int maxFertilizerLevel;
    
    // state
    public SoilData soilData;
    private List<Crop> crops = new List<Crop>();
    private bool tilled;
    public int fertilizerLevel;

    protected override void Start() {
        base.Start();
        tilled = false;
        LoadData();
    }

    public override bool IsInteractable() {
        // can be fertilized
        if (ResourceManager.Instance.carryingFertilizer && fertilizerLevel < maxFertilizerLevel) {
            return true;
        }
        // can be tilled
        else if (crops.Count < maxCrops && !tilled) {
            return true;
        }
        // can plant crops
        else if(tilled && crops.Count < maxCrops && ResourceManager.Instance.HasSeedsLeft()) {
            return true;
        }
        return false;
    }

    public override void Interact() {
        // fertilize
        if (ResourceManager.Instance.carryingFertilizer && fertilizerLevel < maxFertilizerLevel) {
            fertilizerLevel = maxFertilizerLevel;
        }
        // till
        else if (crops.Count < maxCrops && !tilled) {
            tilled = true;
        }
        // plant crops
        else if(tilled && crops.Count < maxCrops && ResourceManager.Instance.HasSeedsLeft()) {
            var lookPosition = CameraRaycast.Instance.GetCurrentHitPosition();
            SpawnCrop(lookPosition);
            
            ResourceManager.Instance.UseSeed();
        }
    }

    public bool ConsumeFertilizer() {
        if (fertilizerLevel <= 0) return false;
        
        // has fertilizer, lower level 
        fertilizerLevel--;
        return true;
    }

    private void SpawnCrop(Vector3 position, Crop.GrowthStage stage = Crop.GrowthStage.Seed) {
        Crop crop = Instantiate(seedPrefab, position, transform.rotation).GetComponent<Crop>();
        crops.Add(crop);
        crop.soil = this;
        crop.stage = stage;
    }

    public void RemoveCrop(Crop crop) {
        crops.Remove(crop);
    }

    public override string GetUIText() {
        string uiText = "";
        if (tilled) uiText += "tilled ";
        if (fertilizerLevel > 0) uiText += "fertilized ";
        uiText += "soil";
        uiText += "\n";

        
        // fertilize
        if(ResourceManager.Instance.carryingFertilizer) {
            if (fertilizerLevel < maxFertilizerLevel) uiText += "E to fertilize";
            else uiText += "already fertilized";
        }
        // plant or till 
        else if (crops.Count < maxCrops) {
            if (tilled) {
                if (ResourceManager.Instance.HasSeedsLeft()) uiText += "E to plant seed";
                else uiText += "out of seeds";
            }
            else {
                uiText += "E to till";
            }
        }
        // no more space
        else {
            uiText += "no more space";
        }

        return uiText;
    }

    public void SaveData() {
        soilData.ClearData();
        foreach (var crop in crops) {
            soilData.SaveCropData(crop);
        }
        soilData.SaveToFile();
    }

    private void LoadData() {
        crops.Clear();
        soilData.LoadDataFromFile();
        foreach (var cropData in soilData.cropData) {
            SpawnCrop(cropData.position, cropData.stage);
        }
    }
}
