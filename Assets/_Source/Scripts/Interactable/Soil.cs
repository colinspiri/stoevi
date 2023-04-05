using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;

public class Soil : Interactable {
    // components
    public GameObject seedPrefab;
    public ASoundContainer crop_plant;
    public ASoundContainer crop_fertilize;
    public SoilData soilData;
    public HeldItem heldItem;
    
    // constants
    public FarmingConstants farmingConstants;
    public IntVariable seeds;

    // state
    public List<Crop> crops = new List<Crop>();
    public bool fertilized { get; private set; }

    protected override void Start() {
        base.Start();
        LoadData();
    }

    public override bool IsInteractable() {
        // can plant crops
        if(crops.Count < farmingConstants.maxCrops && seeds.Value > 0) {
            return true;
        }
        return false;
    }

    public override void Interact() {
        // plant crops
        if(crops.Count < farmingConstants.maxCrops && seeds.Value > 0) {
            var lookPosition = CameraRaycast.Instance.GetCurrentInteractableHitPosition();
            SpawnCrop(lookPosition);
            
            seeds.ApplyChange(-1);
        }
    }

    public void Fertilize() {
        fertilized = true;
        foreach (var crop in crops) {
            crop.Fertilize();
        }
        
        heldItem.RemoveItem();
    }

    public override void OnStartInteracting() {
        base.OnStartInteracting();
        
        /*// fertilizing
        if (!fertilized) {
            crop_fertilize.Play3D(transform);
        }*/
        // planting crops
        if(crops.Count < farmingConstants.maxCrops && seeds.Value > 0) {
            crop_plant.Play3D(transform);
        }
    }

    private void SpawnCrop(Vector3 position, Crop.GrowthStage stage = Crop.GrowthStage.Sprout) {
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
        if (fertilized) return "fertilized";
        else return "";
    }

    public override string GetButtonPrompt() {
        // fertilize
        /*if() {
            if(fertilized) return "already fertilized";
            return "hold " + GetInteractButton() + " to fertilize";
        }*/
        // plant 
        if (seeds.Value <= 0) {
            return "out of seeds";
        }
        else if (crops.Count < farmingConstants.maxCrops) {
            return "hold " + GetInteractButton() + " to plant seed";
        }
        else return "no more space";
    }

    public void SaveData() {
        soilData.SaveDataFromSoil(this);
    }

    public void LoadData() {
        DestroyInstantiatedCrops();

        soilData.LoadDataFromFile();

        foreach (var cropData in soilData.cropData) {
            Vector3 spawnPosition = cropData.relativePosition + transform.position;
            spawnPosition.y = transform.position.y + transform.localScale.y / 2;
            SpawnCrop(spawnPosition, cropData.stage);
        }
    }

    private void DestroyInstantiatedCrops() {
        // destroy all crops in list
        while (crops.Count > 0) {
            if (Application.isEditor) DestroyImmediate(crops[0].gameObject);
            else Destroy(crops[0].gameObject);
            
            if(crops.Count > 0) crops.RemoveAt(0);
        }
        crops.Clear();
        
        // also destroy any crops parented but not referenced
        foreach (Transform child in transform) {
            if (child.GetComponent<Crop>() == null) continue;
            if(Application.isPlaying) Destroy(child.gameObject);
            else DestroyImmediate(child.gameObject);
        }
    }
}
