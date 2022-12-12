using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Crop : Interactable {
    // components
    public SpriteRenderer spriteRenderer;
    public FarmingConstants farmingConstants;
    public Soil soil;

    // constants
    public int maxTomatoes;
    public int minTomatoes;
    public float growTime;

    // state
    public enum CropStage { Seed, Sprout, Intermediate, Unripe, Ripe, Bare }
    public CropStage stage;
    
    // growth
    private bool growing;
    private float growthTimer;
    // watering
    private bool watered;
    // fertilizing
    public bool fertilized;
    // harvesting
    private int tomatoesLeft;
    

    protected override void Start() {
        base.Start();
        
        ChangeCropStage(stage);
        
        tomatoesLeft = Random.Range(minTomatoes, maxTomatoes + 1);
    }

    public override bool IsInteractable() {
        if (stage == CropStage.Seed || stage == CropStage.Sprout || stage == CropStage.Intermediate || stage == CropStage.Unripe) {
            return ResourceManager.Instance.HasWater();
        }

        if (stage == CropStage.Ripe) return true;

        if (stage == CropStage.Bare) return false;

        return false;
    }

    private void Update() {
        if (growing) {
            growthTimer -= Time.deltaTime;
            if(growthTimer <= 0) Grow();
        }
    }

    public override void Interact() {
        if (stage == CropStage.Ripe) {
            Harvest();
            return;
        }

        if (!watered && ResourceManager.Instance.HasWater() && 
            (stage == CropStage.Seed || 
             stage == CropStage.Sprout || 
             stage == CropStage.Intermediate ||
             stage == CropStage.Unripe)) {
            Water();
        }
    }

    private void Water() {
        ResourceManager.Instance.UseWater();
        AudioManager.Instance.PlayWaterSound();

        // if already watered, don't do anything (TODO: over-water?)
        if (watered) return;

        watered = true;

        // start growing
        if (stage == CropStage.Seed || stage == CropStage.Sprout || stage == CropStage.Intermediate ||
            stage == CropStage.Unripe) {
            StartGrowing();
        }
    }

    private void Harvest() {
        ResourceManager.Instance.PlayerHarvestedTomato();
        AudioManager.Instance.PlayHarvestSound();
        RemoveRipeTomatoes();
    }

    public void RemoveRipeTomatoes() {
        tomatoesLeft--;
        watered = false;
        ChangeCropStage(tomatoesLeft <= 0 ? CropStage.Bare : CropStage.Unripe);
    }

    public void AdvanceToNextDay() {
        
    }

    private void StartGrowing() {
        growing = true;
        growthTimer = growTime;
    }

    private void Grow() {
        // check if fertilized
        if (stage == CropStage.Seed || stage == CropStage.Sprout) {
            if (soil != null && soil.ConsumeFertilizer()) {
                fertilized = true;
                Debug.Log(gameObject.name + " is fertilized");
            }
        }
        
        // advance to next growth stage
        if (stage == CropStage.Seed && fertilized) {
            ChangeCropStage(CropStage.Intermediate);
        }
        else {
            ChangeCropStage(stage switch {
                CropStage.Seed => CropStage.Sprout,
                CropStage.Sprout => CropStage.Intermediate,
                CropStage.Intermediate => CropStage.Unripe,
                CropStage.Unripe => CropStage.Ripe,
                _ => stage
            });
        }
        
        // set bools
        growing = false;
        watered = false;
    }

    private void ChangeCropStage(CropStage newStage) {
        stage = newStage;
        
        // change sprite
        spriteRenderer.sprite = stage switch {
            CropStage.Seed => farmingConstants.emptySprite,
            CropStage.Sprout => farmingConstants.waterSprite,
            CropStage.Intermediate => farmingConstants.waterSprite,
            CropStage.Unripe => farmingConstants.waterSprite,
            CropStage.Ripe => farmingConstants.harvestSprite,
            CropStage.Bare => farmingConstants.emptySprite,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override string GetUIText() {
        string uiText = stage.ToString().ToLower();

        uiText += "\n";
        
        switch (stage) {
            case CropStage.Seed:
            case CropStage.Sprout:
            case CropStage.Intermediate:
            case CropStage.Unripe: {
                if (growing) {
                    uiText += (stage == CropStage.Unripe) ? "ripe" : "growing";
                    uiText += " in " + Mathf.Ceil(growthTimer).ToString("0") + "s";
                }
                else {
                    uiText += ResourceManager.Instance.IsWaterEmpty() ? "out of water" : "E to water plant";
                }
                break;
            }
            case CropStage.Ripe:
                uiText += "E to harvest tomato";
                break;
            case CropStage.Bare:
                break;
            default:
                uiText += "ERROR";
                break;
        }

        return uiText;
    }

    public override float GetSliderFloat() {
        return growthTimer / growTime;
    }
}
