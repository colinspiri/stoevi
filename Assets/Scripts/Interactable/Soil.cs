using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using SpookuleleAudio;
using UnityEngine;
using Yarn.Compiler;

public class Soil : Interactable {
    // components
    public GameObject seedPrefab;
    public ASoundContainer crop_plant;
    public ASoundContainer crop_fertilize;
    public SoilData soilData;
    public HeldItem heldItem;
    
    // constants
    public FarmingConstants farmingConstants;
    public Item fertilizer;
    public IntVariable seeds;

    // state
    public List<Crop> crops = new List<Crop>();
    public bool fertilized { get; private set; }

    protected override void Start() {
        base.Start();
        LoadData();
    }

    public override bool IsInteractable() {
        // can be fertilized
        if (!fertilized && heldItem.heldItem == fertilizer) {
            return true;
        }
        // can plant crops
        else if(crops.Count < farmingConstants.maxCrops && seeds.Value > 0) {
            return true;
        }
        return false;
    }

    public override void Interact() {
        // fertilize
        if (!fertilized && heldItem.heldItem == fertilizer) {
            fertilized = true;
            foreach (var crop in crops) {
                crop.Fertilize();
            }
        }
        // plant crops
        else if(crops.Count < farmingConstants.maxCrops && seeds.Value > 0) {
            var lookPosition = CameraRaycast.Instance.GetCurrentInteractableHitPosition();
            SpawnCrop(lookPosition);
            
            seeds.ApplyChange(-1);
        }
    }

    public override void OnStartInteracting() {
        base.OnStartInteracting();
        
        // fertilizing
        if (!fertilized && heldItem.heldItem == fertilizer) {
            crop_fertilize.Play3D(transform);
        }
        // planting crops
        else if(crops.Count < farmingConstants.maxCrops && seeds.Value > 0) {
            crop_plant.Play3D(transform);
        }
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
        if (fertilized) return "fertilized";
        else return "";
    }

    public override string GetButtonPrompt() {
        // fertilize
        if(heldItem.heldItem == fertilizer) {
            if(fertilized) return "already fertilized";
            return GetInteractButton() + " to fertilize";
        }
        // plant 
        else if (crops.Count < farmingConstants.maxCrops) {
            if (seeds.Value > 0) return GetInteractButton() + " to plant seed";
            return "out of seeds";
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
        
        // also destroy anything parented but not referenced
        foreach (Transform child in transform) {
            if(Application.isPlaying) Destroy(child.gameObject);
            else DestroyImmediate(child.gameObject);
        }
    }
}
