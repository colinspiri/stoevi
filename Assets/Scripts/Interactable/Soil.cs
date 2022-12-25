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
    public List<Crop> crops = new List<Crop>();
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
        crop.transform.parent = transform;
        crop.soil = this;
        crop.stage = stage;
        
        crops.Add(crop);
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

    public void SaveData() {
        soilData.SaveDataFromSoil(this);
    }

    public void LoadData() {
        DestroyInstantiatedCrops();
        
        soilData.LoadDataFromFile();
        foreach (var cropData in soilData.cropData) {
            Vector3 spawnPosition = cropData.relativePosition;
            SpawnCrop(spawnPosition, cropData.stage);
        }
    }

    private void DestroyInstantiatedCrops() {
        while (crops.Count > 0) {
            if (Application.isEditor) {
                Debug.Log("destroyed " + crops[0].name);
                DestroyImmediate(crops[0].gameObject);
            }
            else Destroy(crops[0].gameObject);
            
            crops.RemoveAt(0);
        }
        crops.Clear();
    }

    public void ClearData() {
        DestroyInstantiatedCrops();
        
        soilData.ClearData();
    }
}
