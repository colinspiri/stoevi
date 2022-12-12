using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : Interactable {
    // components
    public GameObject seedPrefab;
    
    // constants
    public int maxCrops;
    public int maxFertilizerLevel;
    
    // state
    public SoilData soilData;
    private List<Crop> crops = new List<Crop>();
    public int fertilizerLevel;

    protected override void Start() {
        base.Start();
        LoadData();
    }

    public override bool IsInteractable() {
        if (ResourceManager.Instance.carryingFertilizer) {
            return fertilizerLevel < maxFertilizerLevel;
        }
        else {
            return ResourceManager.Instance.HasSeedsLeft() && crops.Count < maxCrops;
        }
    }

    public override void Interact() {
        // get player look position
        var lookPosition = CameraRaycast.Instance.GetCurrentHitPosition();

        if (ResourceManager.Instance.carryingFertilizer) {
            // fertilize
            fertilizerLevel = maxFertilizerLevel;
        }
        else {
            // use seed
            ResourceManager.Instance.UseSeed();
        
            // spawn crop
            SpawnCrop(lookPosition);
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

    public override string GetUIText() {
        string uiText = fertilizerLevel > 0 ? "fertilized soil" : "soil";
        uiText += "\n";

        if (ResourceManager.Instance.carryingFertilizer) {
            if (IsInteractable()) uiText += "E to fertilize";
        }
        else {
            if (IsInteractable()) uiText += "E to plant seed";
            else if (!ResourceManager.Instance.HasSeedsLeft()) uiText += "out of seeds";
            else if (crops.Count >= maxCrops) uiText += "no more space";
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
