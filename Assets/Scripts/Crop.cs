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
    public Soil soil;

    // constants
    public FarmingConstants farmingConstants;

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
    public int tomatoesYielded;
    private int tomatoesLeft;
    

    protected override void Start() {
        base.Start();

        ChangeCropStage(stage);
    }

    public override bool IsInteractable() {
        // can be removed
        if (stage == GrowthStage.Bare || health == Health.Dead) return true;
        
        // can be watered
        if (stage == GrowthStage.Seed || stage == GrowthStage.Sprout || stage == GrowthStage.Intermediate || stage == GrowthStage.Unripe) {
            return ResourceManager.Instance.HasWater();
        }
        
        // can be harvested
        if (stage == GrowthStage.Ripe) return true;

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
        // can be removed
        if (stage == GrowthStage.Bare || health == Health.Dead) {
            Destroy(gameObject);
            return;
        }
        // can be harvested
        else if (stage == GrowthStage.Ripe) {
            Harvest();
            return;
        }
        // can be watered
        else if (state == State.NeedsWater && ResourceManager.Instance.HasWater() && 
            (stage == GrowthStage.Seed || 
             stage == GrowthStage.Sprout || 
             stage == GrowthStage.Intermediate ||
             stage == GrowthStage.Unripe)) {
            Water();
            return;
        }
        // DEBUG: shorten growth timer
        else if (state == State.Growing) {
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

        // set tomatoes left
        if (stage == GrowthStage.Ripe && tomatoesYielded == 0) {
            tomatoesYielded = 1;
            
            if (health == Health.Fair && fertilized) {
                float rand = UnityEngine.Random.Range(0f, 1f);
                // Debug.Log("rand = " + rand);
                
                if (rand < 0.75f) {
                    tomatoesYielded = 2;
                    
                    float rand2 = UnityEngine.Random.Range(0f, 1f);
                    // Debug.Log("rand2 = " + rand2);
                    
                    if (rand2 < 0.25f) {
                        tomatoesYielded = 3;
                    }
                }
            }
            else if (health == Health.Fair) {
                float rand = UnityEngine.Random.Range(0f, 1f);
                // Debug.Log("rand = " + rand);
                
                if (rand < 0.25f) {
                    tomatoesYielded = 2;
                }
            }

            tomatoesLeft = tomatoesYielded;
            // Debug.Log("yielded " + tomatoesYielded + " tomatoes!");
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
        string uiText = "";
        uiText += stage.ToString().ToLower();
        if (health == Health.Dead) uiText += " (dead)";
        else uiText += " (health: " + health.ToString().ToLower() + ")";

        uiText += "\n";
        
        // can be removed
        if (stage == GrowthStage.Bare || health == Health.Dead) {
            uiText += "E to dig up";
            return uiText;
        }
        
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
        if(state == State.Growing) return growthTimer / growthTime;
        if (state == State.NeedsWater) return thirstyTimer / thirstyTime;
        return 0;
    }

    private string FormatTimer(float timer) {
        int minutes = Mathf.FloorToInt(timer / 60F);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);
        return string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        soil.RemoveCrop(this);
    }
}
