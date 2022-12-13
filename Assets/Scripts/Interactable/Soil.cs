using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Compiler;

public class Soil : Interactable {
    // components
    public GameObject seedPrefab;
    public SoilData soilData;

    // constants
    public FarmingConstants farmingConstants;
    
    // state
    private List<Crop> crops = new List<Crop>();
    private bool tilled;
    private int fertilizerLevel;

    protected override void Start() {
        base.Start();
        tilled = false;
        LoadData();
    }

    public override bool IsInteractable() {
        // can be fertilized
        if (ResourceManager.Instance.carryingFertilizer && fertilizerLevel < farmingConstants.maxFertilizerLevel) {
            return true;
        }
        // can be tilled
        else if (crops.Count < farmingConstants.maxCrops && !tilled) {
            return true;
        }
        // can plant crops
        else if(tilled && crops.Count < farmingConstants.maxCrops && ResourceManager.Instance.HasSeedsLeft()) {
            return true;
        }
        return false;
    }

    public override void Interact() {
        // fertilize
        if (ResourceManager.Instance.carryingFertilizer && fertilizerLevel < farmingConstants.maxFertilizerLevel) {
            fertilizerLevel = farmingConstants.maxFertilizerLevel;
        }
        // till
        else if (crops.Count < farmingConstants.maxCrops && !tilled) {
            tilled = true;
        }
        // plant crops
        else if(tilled && crops.Count < farmingConstants.maxCrops && ResourceManager.Instance.HasSeedsLeft()) {
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
    
    public override string GetObjectName() {
        return "soil";
    }

    public override string GetObjectDescription() {
        if (tilled && fertilizerLevel > 0) return "tilled & fertilized";
        else if (tilled) return "tilled";
        else if (fertilizerLevel > 0) return "fertilized";
        else return "";
    }

    public override string GetButtonPrompt() {
        // fertilize
        if(ResourceManager.Instance.carryingFertilizer) {
            if (fertilizerLevel < farmingConstants.maxFertilizerLevel) return "E to fertilize";
            return "already fertilized";
        }
        // plant or till 
        else if (crops.Count < farmingConstants.maxCrops) {
            if (tilled) {
                if (ResourceManager.Instance.HasSeedsLeft()) return "E to plant seed";
                return "out of seeds";
            }
            else {
                return "E to till";
            }
        }
        // no more space
        else {
            return "no more space";
        }
    }

    public override string GetUIText() {
        string uiText = "";
        if (tilled) uiText += "tilled ";
        if (fertilizerLevel > 0) uiText += "fertilized ";
        uiText += "soil";
        uiText += "\n";

        
        // fertilize
        if(ResourceManager.Instance.carryingFertilizer) {
            if (fertilizerLevel < farmingConstants.maxFertilizerLevel) uiText += "E to fertilize";
            else uiText += "already fertilized";
        }
        // plant or till 
        else if (crops.Count < farmingConstants.maxCrops) {
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
