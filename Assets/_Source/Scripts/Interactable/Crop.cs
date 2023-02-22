using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Movement;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityString;
using SpookuleleAudio;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Compiler;
using Random = UnityEngine.Random;

public class Crop : Interactable {
    // components
    public CropTextureManager textureManager;
    public GameObject cover;
    public Soil soil;
    public ASoundContainer crop_water;
    public ASoundContainer crop_fertilize;
    public ASoundContainer item_pickup;

    // constants
    public FarmingConstants farmingConstants;
    public float sproutHeight;
    public float halfHeight;
    public float fullHeight;
    
    // shared state
    public IntVariable seeds;
    public IntVariable currentWater;
    public IntVariable playerTomatoes;
    public HeldItem heldItem;
    public Item fertilizer;

    // state
    public enum GrowthStage { Sprout, Intermediate, Unripe, Ripe, Bare }
    public GrowthStage stage;
    public enum State { NeedsWater, Growing, Harvestable }
    private State state;
    public enum Health { Dead, Wilted, Fair, Great }
    private Health health;
    // growth
    private float growthTime;
    private float growthTimer;
    // watering
    private float thirstyTime;
    private float thirstyTimer;

    private void Awake() {
        textureManager = GetComponent<CropTextureManager>();
    }

    protected override void Start() {
        base.Start();
        health = Health.Fair;
        ChangeCropStage(stage);
    }

    public override bool IsInteractable() {
        // can be removed
        if (stage == GrowthStage.Bare || health == Health.Dead) return true;
        
        // can be watered
        if (state == State.NeedsWater && currentWater.Value > 0 &&
            (stage == GrowthStage.Sprout ||
             stage == GrowthStage.Intermediate ||
             stage == GrowthStage.Unripe)) {
            return true;
        }

        // can be harvested
        if (stage == GrowthStage.Ripe) return true;
        
        // fertilize
        if (soil != null && !soil.fertilized && heldItem.heldItem == fertilizer) return true;
        
        // DEBUG: shorten growth timer
        #if true && UNITY_EDITOR
        if (state == State.Growing) {
            return true;
        }
        #endif

        return false;
    }

    private void Update() {
        // growth timer
        if (state == State.Growing) {
            growthTimer -= Time.deltaTime;
            if(growthTimer <= 0) Grow();
        }
        // thirsty timer
        else if (state == State.NeedsWater) {
            thirstyTimer -= Time.deltaTime;
            if (thirstyTimer <= 0) {
                if (health == Health.Fair) {
                    health = Health.Wilted;
                    textureManager.UpdateTextures(stage, state, health);
                    UpdateCover();
                    
                    StartThirsty();
                }
                else if (health == Health.Wilted) {
                    health = Health.Dead;
                    textureManager.UpdateTextures(stage, state, health);
                    UpdateCover();
                }
            }
        }
    }

    public void Fertilize() {
        // reduce grow timer
        if (state == State.Growing) {
            float reductionProportion = (farmingConstants.baseGrowthTime - farmingConstants.fertilizedGrowthTime) / farmingConstants.baseGrowthTime;
            growthTimer -= growthTimer * reductionProportion;
        }

        // increase health
        health = health switch {
            Health.Dead => Health.Dead,
            Health.Wilted => Health.Fair,
            Health.Fair => Health.Great,
        };
    }

    public override void Interact() {
        // can be removed
        if (stage == GrowthStage.Bare || health == Health.Dead) {
            seeds.ApplyChange(1);
            item_pickup.Play();
            AudioManager.Instance.PlayHarvestSound();
            Destroy(gameObject);
            return;
        }
        // can be harvested
        if (stage == GrowthStage.Ripe) {
            Harvest();
            return;
        }
        // can be watered
        if (state == State.NeedsWater && currentWater.Value > 0 && 
                 (stage == GrowthStage.Sprout || 
                  stage == GrowthStage.Intermediate ||
                  stage == GrowthStage.Unripe)) {
            Water();
            return;
        }
        // fertilize
        if (soil != null && !soil.fertilized && heldItem.heldItem == fertilizer) {
            soil.Fertilize();
            return;
        }
        // DEBUG: shorten growth timer
        #if true && UNITY_EDITOR
        if (state == State.Growing) {
            growthTimer = 2f;
            return;
        }
        #endif
    }
    
    public override void OnStartInteracting() {
        base.OnStartInteracting();
        
        // if starting to water
        if (state == State.NeedsWater && currentWater.Value > 0) {
            crop_water.Play3D(transform);
        }
        // fertilize
        else if (soil != null && !soil.fertilized && heldItem.heldItem == fertilizer) {
            crop_fertilize.Play3D(transform);
        }
    }

    private void Water() {
        currentWater.ApplyChange(-1);
        
        // if already watered, don't do anything (TODO: over-water?)
        if (state != State.NeedsWater) return;

        // start growing
        if (stage == GrowthStage.Sprout || stage == GrowthStage.Intermediate ||
            stage == GrowthStage.Unripe) {
            StartGrowing();
        }
        
        // update sprite
        textureManager.UpdateTextures(stage, state, health);
        UpdateCover();
    }

    private void StartGrowing() {
        state = State.Growing;

        growthTime = (soil && soil.fertilized) ? farmingConstants.fertilizedGrowthTime : farmingConstants.baseGrowthTime;
        growthTimer = growthTime;
    }

    private void StartThirsty() {
        state = State.NeedsWater;
        
        thirstyTime = (soil && soil.fertilized) ? farmingConstants.fertilizedThirstyTime : farmingConstants.baseThirstyTime;
        thirstyTimer = thirstyTime;
    }

    private void Harvest() {
        AudioManager.Instance.PlayHarvestSound();
        
        ChangeCropStage(GrowthStage.Unripe);
        
        // calculate number of tomatoes to produce
        int minTomatoes = 0;
        int maxTomatoes = 0;
        // set min/max tomatoes based on health
        if (health == Health.Great) {
            minTomatoes = 2;
            maxTomatoes = 5;
        }
        else if (health == Health.Fair) {
            minTomatoes = 1;
            maxTomatoes = 3;
        }
        else if (health == Health.Wilted) {
            minTomatoes = 1;
            maxTomatoes = 1;
        }
        var tomatoesYielded = Random.Range(minTomatoes, maxTomatoes);
        playerTomatoes.ApplyChange(tomatoesYielded);
    }

    public void Destroy() {
        ChangeCropStage(GrowthStage.Bare);
    }


    public void AdvanceToNextDay() {
        
    }

    private void Grow() {
        ChangeCropStage(stage switch {
            GrowthStage.Sprout => GrowthStage.Intermediate,
            GrowthStage.Intermediate => GrowthStage.Unripe,
            GrowthStage.Unripe => GrowthStage.Ripe,
            _ => stage
        });
        
        textureManager.UpdateTextures(stage, state, health);
    }

    private void ChangeCropStage(GrowthStage newStage) {
        stage = newStage;

        // set state based on new stage
        switch (stage) {
            case GrowthStage.Sprout:
            case GrowthStage.Intermediate:
            case GrowthStage.Unripe:
                StartThirsty();
                break;
            case GrowthStage.Ripe:
                state = State.Harvestable;
                break;
            case GrowthStage.Bare:
                state = State.Harvestable;
                break;
        }
        
        // update sprite
        textureManager.UpdateTextures(stage, state, health);
        UpdateCover();
    }
    
    private void UpdateCover() {
        float newHeight = stage switch {
            GrowthStage.Sprout => sproutHeight,
            GrowthStage.Intermediate => halfHeight,
            GrowthStage.Unripe => fullHeight,
            GrowthStage.Ripe => fullHeight,
            GrowthStage.Bare => fullHeight,
            _ => 1f
        };

        transform.localScale = new Vector3(transform.localScale.x, (1/soil.transform.localScale.y) * newHeight, transform.localScale.z);

        // disable cover for sprouts
        if (stage == GrowthStage.Sprout) {
            cover.SetActive(false);
        }
        else cover.SetActive(true);
    }
    
    public override string GetObjectName() {
        string uiText = stage.ToString().ToLower();
        return uiText;
    }

    public override string GetObjectDescription() {
        if (health == Health.Dead) {
            return "dead";
        }
        else if (health == Health.Wilted) {
            return "wilted";
        }
        else if (soil && soil.fertilized) {
            return "fertilized";
        }

        return "";
    }

    public override string GetButtonPrompt() {
        if (stage == GrowthStage.Bare || health == Health.Dead) {
            return "hold " + GetInteractButton() + " to dig up";
        }
        else if (state == State.NeedsWater) {
            return currentWater.Value <= 0 ? "out of water" : "hold " + GetInteractButton() + " to water";
        }
        else if (stage == GrowthStage.Ripe) {
            return "hold " + GetInteractButton() + " to harvest tomato";
        }
        // fertilize
        else if (soil != null && !soil.fertilized && heldItem.heldItem == fertilizer) {
            return "hold " + GetInteractButton() + " to fertilize";
        }
        return "";
    }

    public override float GetTimerValue() {
        if(state == State.Growing) return 1 - (growthTimer / growthTime);
        if (state == State.NeedsWater) return 1 - (thirstyTimer / thirstyTime);
        return 0;
    }
    public override float GetTimerTime() {
        if (state == State.Growing) return growthTimer;
        if (state == State.NeedsWater) return thirstyTimer;
        return 0;
    }
    public override InteractableUI.TimerIcon GetTimerIcon() {
        if (state == State.Growing) {
            if (stage == GrowthStage.Unripe) return InteractableUI.TimerIcon.Ripe;
            else return InteractableUI.TimerIcon.Growth;
        }
        if (state == State.NeedsWater) return InteractableUI.TimerIcon.Water;
        return InteractableUI.TimerIcon.None;
    }

    protected void OnDestroy() {
        soil.RemoveCrop(this);
    }
}
