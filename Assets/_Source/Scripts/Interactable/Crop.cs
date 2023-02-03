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

public class Crop : Interactable {
    // components
    public List<SpriteRenderer> spriteRenderers;
    public GameObject cover;
    public Soil soil;
    public ASoundContainer crop_water;

    // constants
    public FarmingConstants farmingConstants;
    public float seedHeight;
    public float sproutHeight;
    public float halfHeight;
    public float fullHeight;
    
    // shared state
    public IntVariable seeds;
    public IntVariable currentWater;
    public IntVariable playerTomatoes;
    public TimeOfDay timeOfDay;

    // state
    public enum GrowthStage { Seed, Sprout, Intermediate, Unripe, Ripe, Bare }
    public GrowthStage stage;
    private enum State { NeedsWater, Growing, Harvestable }
    private State state;
    private enum Health { Dead, Poor, Fair, Great }
    private Health health;
    // growth
    private float growthTime;
    private float growthTimer;
    // watering
    private float thirstyTime;
    private float thirstyTimer;
    // harvesting
    public int tomatoesYielded;
    private int tomatoesLeft;
    
    protected override void Start() {
        base.Start();
        health = Health.Fair;
        ChangeCropStage(stage);
    }

    public override bool IsInteractable() {
        // can be removed
        if (stage == GrowthStage.Bare || health == Health.Dead) return true;
        
        // can be watered
        if (!timeOfDay.IsNight() && state == State.NeedsWater && currentWater.Value > 0 &&
            (stage == GrowthStage.Seed ||
             stage == GrowthStage.Sprout ||
             stage == GrowthStage.Intermediate ||
             stage == GrowthStage.Unripe)) {
            return true;
        }

        // can be harvested
        if (stage == GrowthStage.Ripe) return true;
        
        // DEBUG: shorten growth timer
        #if UNITY_EDITOR
        if (state == State.Growing) {
            return true;
        }
        #endif

        return false;
    }

    private void Update() {
        // growth timer
        if (state == State.Growing && !timeOfDay.IsNight()) {
            growthTimer -= Time.deltaTime;
            if(growthTimer <= 0) Grow();
        }
        // thirsty timer
        else if (state == State.NeedsWater && !timeOfDay.IsNight()) {
            thirstyTimer -= Time.deltaTime;
            if (thirstyTimer <= 0) {
                if (health == Health.Fair) {
                    health = Health.Poor;
                    UpdateSprite();
                    UpdateCover();
                    
                    StartThirsty();
                }
                else if (health == Health.Poor) {
                    health = Health.Dead;
                    UpdateSprite();
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
            Health.Poor => Health.Fair,
            Health.Fair => Health.Great,
        };
    }

    public override void Interact() {
        // can be removed
        if (stage == GrowthStage.Bare || health == Health.Dead) {
            seeds.ApplyChange(1);
            AudioManager.Instance.PlayHarvestSound();
            Destroy(gameObject);
            return;
        }
        // can be harvested
        else if (stage == GrowthStage.Ripe) {
            Harvest();
            return;
        }
        // can be watered
        else if (!timeOfDay.IsNight() && state == State.NeedsWater && currentWater.Value > 0 && 
                 (stage == GrowthStage.Seed || 
                  stage == GrowthStage.Sprout || 
                  stage == GrowthStage.Intermediate ||
                  stage == GrowthStage.Unripe)) {
            Water();
            return;
        }
        // DEBUG: shorten growth timer
        #if UNITY_EDITOR
        else if (state == State.Growing) {
            growthTimer = 2f;
            return;
        }
        #endif
    }
    
    public override void OnStartInteracting() {
        base.OnStartInteracting();
        // if starting to water
        if (!timeOfDay.IsNight() && state == State.NeedsWater && currentWater.Value > 0) {
            crop_water.Play3D(transform);
        }
    }

    private void Water() {
        currentWater.ApplyChange(-1);
        
        // if already watered, don't do anything (TODO: over-water?)
        if (state != State.NeedsWater) return;

        // start growing
        if (stage == GrowthStage.Seed || stage == GrowthStage.Sprout || stage == GrowthStage.Intermediate ||
            stage == GrowthStage.Unripe) {
            StartGrowing();
        }
        
        
        // update sprite
        UpdateSprite();
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
        playerTomatoes.ApplyChange(1);
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
        // advance to next growth stage
        if (stage == GrowthStage.Seed) {
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
        
        UpdateSprite();
    }

    private void ChangeCropStage(GrowthStage newStage) {
        stage = newStage;

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
        
        // update sprite
        UpdateSprite();
        UpdateCover();

        // set tomatoes left
        if (stage == GrowthStage.Ripe && tomatoesYielded == 0) {
            tomatoesYielded = 1;
            
            if (health == Health.Great) {
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
        }
    }

    private void UpdateSprite() {
        Sprite newSprite = null;
        
        if (health == Health.Dead) {
            newSprite = farmingConstants.emptySprite;
        }
        else {
            switch (stage) {
                case GrowthStage.Seed:
                case GrowthStage.Sprout:
                case GrowthStage.Intermediate:
                case GrowthStage.Unripe:
                    if (state == State.NeedsWater) newSprite = farmingConstants.emptySprite;
                    else if (state == State.Growing) newSprite = farmingConstants.waterSprite;
                    break;
                case GrowthStage.Ripe:
                    newSprite = farmingConstants.harvestSprite;
                    break;
                case GrowthStage.Bare:
                    newSprite = farmingConstants.emptySprite;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // set all renderers
        foreach (var spriteRenderer in spriteRenderers) {
            spriteRenderer.sprite = newSprite;
        }
        
    }
    private void UpdateCover() {
        float newHeight = stage switch {
            GrowthStage.Seed => seedHeight,
            GrowthStage.Sprout => sproutHeight,
            GrowthStage.Intermediate => halfHeight,
            GrowthStage.Unripe => fullHeight,
            GrowthStage.Ripe => fullHeight,
            GrowthStage.Bare => fullHeight,
            _ => 1f
        };

        transform.localScale = new Vector3(transform.localScale.x, (1/soil.transform.localScale.y) * newHeight, transform.localScale.z);

        // disable cover for seeds or sprouts
        if (stage == GrowthStage.Seed || stage == GrowthStage.Sprout) {
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
        else if (health == Health.Poor) {
            return "wilted";
        }
        else if (soil && soil.fertilized) {
            return "fertilized";
        }

        return "";
    }

    public override string GetButtonPrompt() {
        if (stage == GrowthStage.Bare || health == Health.Dead) {
            return GetInteractButton() + " to dig up";
        }
        else if (timeOfDay.IsNight()) {
            return "";
        }
        else if (state == State.NeedsWater) {
            return currentWater.Value <= 0 ? "out of water" : GetInteractButton() + " to water";
        }
        else if (stage == GrowthStage.Ripe) {
            return GetInteractButton() + " to harvest tomato";
        }
        return "";
    }

    public override float GetTimerValue() {
        if (timeOfDay.IsNight()) return 0;
        if(state == State.Growing) return 1 - (growthTimer / growthTime);
        if (state == State.NeedsWater) return 1 - (thirstyTimer / thirstyTime);
        return 0;
    }
    public override float GetTimerTime() {
        if (timeOfDay.IsNight()) return 0;
        if (state == State.Growing) return growthTimer;
        if (state == State.NeedsWater) return thirstyTimer;
        return 0;
    }
    public override InteractableUI.TimerIcon GetTimerIcon() {
        if (timeOfDay.IsNight()) return InteractableUI.TimerIcon.None;
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