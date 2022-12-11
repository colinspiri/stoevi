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

    private void Update() {
        SetInteractable(true);
        SetSelectable(true);
    }

    public override void Interact() {
        // get player look position
        var lookPosition = CameraRaycast.Instance.GetCurrentHitPosition();

        if (ResourceManager.Instance.HasSeedsLeft() && crops.Count < maxCrops) {
            // use seed
            ResourceManager.Instance.UseSeed();
        
            // spawn crop
            SpawnCrop(lookPosition);
        }
        else {
            // fertilize
            fertilizerLevel = maxFertilizerLevel;
        }

    }

    public bool ConsumeFertilizer() {
        if (fertilizerLevel <= 0) return false;
        
        // has fertilizer, lower level 
        fertilizerLevel--;
        return true;
    }

    private void SpawnCrop(Vector3 position, Crop.CropStage stage = Crop.CropStage.Seed) {
        Crop crop = Instantiate(seedPrefab, position, transform.rotation).GetComponent<Crop>();
        crops.Add(crop);
        crop.soil = this;
        crop.stage = stage;
    }

    public override string GetUIText() {
        // if (crops.Count >= maxCrops) return "no more space";
        if (!ResourceManager.Instance.HasSeedsLeft()) return "out of seeds";
        return "E to plant seed";
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
