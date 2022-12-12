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
    private enum Health { Fair, Poor, Dead }
    private Health health;
    // growth
    private int timesGrownToday;
    private float growthTime;
    private float growthTimer;
    // watering
    private float thirstyTime;
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
        if (health == Health.Dead) return false;
        
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
                if (health == Health.Fair) {
                    health = Health.Poor;
                    UpdateSprite();
                    
                    timesGrownToday++;
                    StartThirsty();
                }
                else if (health == Health.Poor) {
                    health = Health.Dead;
                    UpdateSprite();
                }
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
            return;
        }
        
        if (state == State.Growing) {
            growthTimer = 2f;
            return;
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
            StartGrowing();
        }
    }

    private void StartGrowing() {
        state = State.Growing;

        float growthTimeByStage = farmingConstants.growthTimeByStage[stage];

        // get multiplier
        var index = timesGrownToday >= farmingConstants.consecutiveTimerMultipliers.Count
            ? farmingConstants.consecutiveTimerMultipliers.Count - 1
            : timesGrownToday;
        float multiplier = farmingConstants.consecutiveTimerMultipliers[index];

        // set growth timer
        growthTime = growthTimeByStage * multiplier;
        growthTimer = growthTime;

        Debug.Log("growth time is " + growthTime + " (" + growthTimeByStage + " * " + multiplier + ")");
    }

    private void StartThirsty() {
        state = State.NeedsWater;

        // get multiplier
        var index = timesGrownToday >= farmingConstants.consecutiveTimerMultipliers.Count
            ? farmingConstants.consecutiveTimerMultipliers.Count - 1
            : timesGrownToday;
        float multiplier = farmingConstants.consecutiveTimerMultipliers[index];
        
        // set thirsty timer
        thirstyTime = farmingConstants.baseThirstyTime * multiplier;
        thirstyTimer = thirstyTime;
        
        Debug.Log("thirsty time is " + thirstyTime + " (" + farmingConstants.baseThirstyTime + " * " + multiplier + ")");
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
        timesGrownToday++;
        
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
    }

    private void ChangeCropStage(GrowthStage newStage) {
        stage = newStage;
        
        UpdateSprite();

        // set state based on new stage
        switch (stage) {
            case GrowthStage.Seed:
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
    }

    private void UpdateSprite() {
        if (health == Health.Dead) {
            spriteRenderer.sprite = farmingConstants.emptySprite;
        }
        
        spriteRenderer.sprite = stage switch {
            GrowthStage.Seed => farmingConstants.emptySprite,
            GrowthStage.Sprout => farmingConstants.waterSprite,
            GrowthStage.Intermediate => farmingConstants.waterSprite,
            GrowthStage.Unripe => farmingConstants.waterSprite,
            GrowthStage.Ripe => farmingConstants.harvestSprite,
            GrowthStage.Bare => farmingConstants.emptySprite,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override string GetUIText() {
        string uiText = stage.ToString().ToLower();
        if (health == Health.Dead) {
            return uiText + " (dead)";
        }
        uiText += " (health: " + health.ToString().ToLower() + ")";

        uiText += "\n";
        
        switch (stage) {
            case GrowthStage.Seed:
            case GrowthStage.Sprout:
            case GrowthStage.Intermediate:
            case GrowthStage.Unripe: {
                if (state == State.Growing) {
                    uiText += (stage == GrowthStage.Unripe) ? "ripe" : "growing";
                    uiText += " in " + FormatTimer(growthTimer);
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
        if (state == State.NeedsWater) return thirstyTimer / thirstyTime;
        return 0;
    }

    private string FormatTimer(float timer) {
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }
}
