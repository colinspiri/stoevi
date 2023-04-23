using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;
using UnityEngine.Serialization;

public class Soil : Interactable {
    [Header("Components")]
    public GameObject seedPrefab;
    
    [Header("Audio")]
    public AudioSource crop_plant;
    public AudioSource crop_fertilize;
    
    [Header("Scriptable Objects")]
    public SoilData soilData;
    public FarmingConstants farmingConstants;
    [FormerlySerializedAs("seeds")] public IntVariable currentSeeds;
    public IntVariable currentFertilizer;

    [Header("State")]
    public List<Crop> crops = new List<Crop>();
    public bool fertilized { get; private set; }

    protected override void Start() {
        base.Start();
        LoadData();
    }

    public override bool IsInteractablePrimary() {
        if(crops.Count < farmingConstants.maxCrops && currentSeeds.Value > 0) {
            return true;
        }
        return false;
    }
    public override void InteractPrimary() {
        if(crops.Count < farmingConstants.maxCrops && currentSeeds.Value > 0) {
            if(ObjectiveUI.Instance != null) ObjectiveUI.Instance.FinishPrompt("Plant");

            currentSeeds.ApplyChange(-1);
            
            var lookPosition = CameraRaycast.Instance.GetCurrentInteractableHitPosition();
            SpawnCrop(lookPosition);
        }
    }
    public override void OnStartInteractingPrimary() {
        base.OnStartInteractingPrimary();
        
        if(crops.Count < farmingConstants.maxCrops && currentSeeds.Value > 0) {
            crop_plant.Play();
            this.InteractionTimePrimary = crop_plant.clip.length;
        }
    }

    public override void OnStopInteractingPrimary() {
        base.OnStopInteractingPrimary();
        
        crop_plant.Stop();

        this.InteractionTimePrimary = 1;
    }

    public override bool IsInteractableSecondary() {
        if (!fertilized && currentFertilizer.Value > 0) {
            return true;
        }

        return false;
    }
    public override void InteractSecondary() {
        base.InteractSecondary();
        
        if (!fertilized && currentFertilizer.Value > 0) {
            currentFertilizer.ApplyChange(-1);
            Fertilize();
        }
    }
    public override void OnStartInteractingSecondary() {
        base.OnStartInteractingSecondary();
        
        if (!fertilized && currentFertilizer.Value > 0) {
            crop_fertilize.Play();
            this.InteractionTimeSecondary = crop_fertilize.clip.length;
        }
    }

    public override void OnStopInteractingSecondary() {
        base.OnStopInteractingSecondary();
        
        crop_fertilize.Stop();

        this.InteractionTimeSecondary = 1;
    }

    public void Fertilize() {
        fertilized = true;
        foreach (var crop in crops) {
            crop.Fertilize();
        }
    }

    private void SpawnCrop(Vector3 position, Crop.GrowthStage stage = Crop.GrowthStage.Sprout, Crop.Health health = Crop.Health.Fair) {
        Crop crop = Instantiate(seedPrefab, position, transform.rotation).GetComponent<Crop>();
        crop.transform.parent = transform;
        crop.soil = this;
        crop.stage = stage;
        crop.health = health;
        
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

    public override string GetButtonPromptPrimary() {
        if (crops.Count >= farmingConstants.maxCrops) {
            return "";
        }
        if (currentSeeds.Value <= 0) {
            return "out of seeds";
        }
        if (crops.Count < farmingConstants.maxCrops && currentSeeds.Value > 0) {
            return GetInteractPrimaryButton() + " plant seed";
        }
        return "";
    }

    public override string GetButtonPromptSecondary() {
        if (currentFertilizer.Value <= 0) {
            return "out of fertilizer";
        }
        else if (!fertilized && currentFertilizer.Value > 0) {
            return GetInteractSecondaryButton() + " fertilize";
        }
        else return "";
    }

    public void SaveData() {
        soilData.SaveDataFromSoil(this);
    }

    public void LoadData() {
        DestroyInstantiatedCrops();

        soilData.LoadDataFromFile();

        foreach (var cropData in soilData.cropData) {
            Vector3 spawnPosition = transform.position;
            spawnPosition.y = transform.position.y + transform.localScale.y / 2;
            SpawnCrop(spawnPosition, cropData.stage, cropData.health);
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
