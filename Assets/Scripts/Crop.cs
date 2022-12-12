using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
    public enum GrowthStage { Seed, Sprout, Intermediate, Unripe, Ripe, Bare }
    public GrowthStage stage;
    private enum State { NeedsWater, Growing, Harvestable }
    private State state;
    // growth
    private float growthTimer;
    // watering
    private float thirstyTimer;
    // fertilizing
    public bool fertilized;
    // harvesting
    private int tomatoesLeft;
    

    protected override void Start() {
        base.Start();
        
        ChangeCropStage(stage);

        tomatoesLeft = UnityEngine.Random.Range(minTomatoes, maxTomatoes + 1);
    }

    public override bool IsInteractable() {
        if (stage == GrowthStage.Seed || stage == GrowthStage.Sprout || stage == GrowthStage.Intermediate || stage == GrowthStage.Unripe) {
            return ResourceManager.Instance.HasWater();
        }

        if (stage == GrowthStage.Ripe) return true;

        if (stage == GrowthStage.Bare) return false;

        return false;
    }

    private void Update() {
        if (state == State.Growing) {
            growthTimer -= Time.deltaTime;
            if(growthTimer <= 0) Grow();
        }
        else if (state == State.NeedsWater) {
            thirstyTimer -= Time.deltaTime;
            if (thirstyTimer <= 0) {
                // TODO: crop health becomes poor OR crop dies if health is already poor
            }
        }
    }

    public override void Interact() {
        if (stage == GrowthStage.Ripe) {
            Harvest();
            return;
        }

        if (state == State.NeedsWater && ResourceManager.Instance.HasWater() && 
            (stage == GrowthStage.Seed || 
             stage == GrowthStage.Sprout || 
             stage == GrowthStage.Intermediate ||
             stage == GrowthStage.Unripe)) {
            Water();
        }
    }

    private void Water() {
        ResourceManager.Instance.UseWater();
        AudioManager.Instance.PlayWaterSound();

        // if already watered, don't do anything (TODO: over-water?)
        if (state != State.NeedsWater) return;

        // start growing
        if (stage == GrowthStage.Seed || stage == GrowthStage.Sprout || stage == GrowthStage.Intermediate ||
            stage == GrowthStage.Unripe) {
            state = State.Growing;
            growthTimer = growTime;
        }
    }

    private void Harvest() {
        ResourceManager.Instance.PlayerHarvestedTomato();
        AudioManager.Instance.PlayHarvestSound();
        RemoveRipeTomatoes();
    }

    public void RemoveRipeTomatoes() {
        tomatoesLeft--;
        
        ChangeCropStage(tomatoesLeft <= 0 ? GrowthStage.Bare : GrowthStage.Unripe);
    }

    public void AdvanceToNextDay() {
        
    }

    private void Grow() {
        // check if fertilized
        if (stage == GrowthStage.Seed || stage == GrowthStage.Sprout) {
            if (soil != null && soil.ConsumeFertilizer()) {
                fertilized = true;
                Debug.Log(gameObject.name + " is fertilized");
            }
        }
        
        // advance to next growth stage
        if (stage == GrowthStage.Seed && fertilized) {
            ChangeCropStage(GrowthStage.Intermediate);
        }
        else {
            ChangeCropStage(stage switch {
                GrowthStage.Seed => GrowthStage.Sprout,
                GrowthStage.Sprout => GrowthStage.Intermediate,
                GrowthStage.Intermediate => GrowthStage.Unripe,
                GrowthStage.Unripe => GrowthStage.Ripe,
                _ => stage
            });
        }
        
        // set bools
        thirstyTimer = farmingConstants.thirstyTime;
    }

    private void ChangeCropStage(GrowthStage newStage) {
        stage = newStage;
        
        // change sprite
        spriteRenderer.sprite = stage switch {
            GrowthStage.Seed => farmingConstants.emptySprite,
            GrowthStage.Sprout => farmingConstants.waterSprite,
            GrowthStage.Intermediate => farmingConstants.waterSprite,
            GrowthStage.Unripe => farmingConstants.waterSprite,
            GrowthStage.Ripe => farmingConstants.harvestSprite,
            GrowthStage.Bare => farmingConstants.emptySprite,
            _ => throw new ArgumentOutOfRangeException()
        };


        // set state based on new stage
        switch (stage) {
            case GrowthStage.Seed:
            case GrowthStage.Sprout:
            case GrowthStage.Intermediate:
            case GrowthStage.Unripe:
                state = State.NeedsWater;
                thirstyTimer = farmingConstants.thirstyTime;
                break;
            case GrowthStage.Ripe:
                state = State.Harvestable;
                break;
            case GrowthStage.Bare:
                state = State.Harvestable;
                break;
        }
    }

    public override string GetUIText() {
        string uiText = stage.ToString().ToLower();

        uiText += "\n";
        
        switch (stage) {
            case GrowthStage.Seed:
            case GrowthStage.Sprout:
            case GrowthStage.Intermediate:
            case GrowthStage.Unripe: {
                if (state == State.Growing) {
                    uiText += (stage == GrowthStage.Unripe) ? "ripe" : "growing";
                    uiText += " in " + Mathf.Ceil(growthTimer).ToString("0") + "s";
                }
                else if(state == State.NeedsWater) {
                    /*uiText += "thirsty in " + Mathf.Ceil(thirstyTimer).ToString("0") + "s";
                    uiText += "\n";*/
                    uiText += ResourceManager.Instance.IsWaterEmpty() ? "out of water" : "E to water plant";
                }
                break;
            }
            case GrowthStage.Ripe:
                uiText += "E to harvest tomato";
                break;
            case GrowthStage.Bare:
                break;
            default:
                uiText += "ERROR";
                break;
        }

        return uiText;
    }

    public override float GetSliderFloat() {
        if(state == State.Growing) return growthTimer / growTime;
        if (state == State.NeedsWater) return thirstyTimer / farmingConstants.thirstyTime;
        return 0;
    }
}
