using SpookuleleAudio;
using UnityEngine;
using Random = UnityEngine.Random;

public class Crop : Interactable {
    // components
    [Header("Components")]
    public CropTextureManager textureManager;
    public CropCoverManager coverManager;
    public CropMapIcon mapIcon;
    public Soil soil;
    
    [Header("Audio")]
    public AudioSource crop_water;
    public AudioSource crop_harvest;
    public AudioSource crop_fertilize;
    public ASoundContainer item_pickup;

    // constants
    [Header("Scriptable Objects")]
    public FarmingConstants farmingConstants;
    
    // shared state
    public IntVariable seeds;
    public IntVariable currentWater;
    public IntVariable currentFertilizer;
    public IntVariable playerTomatoes;
    public GameEvent onHarvest;

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
    
    private void Update() {
        // growth timer
        if (state == State.Growing) {
            growthTimer -= Time.deltaTime;
            if(growthTimer <= 0) Grow();
            
            // update map icon
            mapIcon.UpdateTimer(1 - (growthTimer / growthTime));
        }
        // thirsty timer
        else if (state == State.NeedsWater) {
            thirstyTimer -= Time.deltaTime;
            if (thirstyTimer <= 0) {
                if (health == Health.Fair) {
                    health = Health.Wilted;
                    textureManager.UpdateTextures(stage, state, health);
                    coverManager.UpdateCover(stage);
                    
                    StartThirsty();
                }
                else if (health == Health.Wilted) {
                    health = Health.Dead;
                    textureManager.UpdateTextures(stage, state, health);
                    coverManager.UpdateCover(stage);
                }
            }
        }
    }


    public override bool IsInteractablePrimary() {
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

        // DEBUG: shorten growth timer
        #if true && UNITY_EDITOR
        if (state == State.Growing) {
            return true;
        }
        #endif

        return false;
    }

    public override void InteractPrimary() {
        // can be removed
        if (stage == GrowthStage.Bare || health == Health.Dead) {
            seeds.ApplyChange(1);
            item_pickup.Play();
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
        // DEBUG: shorten growth timer
        #if true && UNITY_EDITOR
        if (state == State.Growing) {
            growthTimer = 2f;
            return;
        }
        #endif
    }

    public override void OnStartInteractingPrimary() {
        base.OnStartInteractingPrimary();
        
        // if starting to water
        if (state == State.NeedsWater && currentWater.Value > 0) {
            crop_water.Play();
            this.InteractionTimePrimary = crop_water.clip.length;
        }
        // harvest or dig up bare
        else if (stage == GrowthStage.Ripe || stage == GrowthStage.Bare || health == Health.Dead) {
            crop_harvest.Play();
            this.InteractionTimePrimary = crop_harvest.clip.length;
        }
    }
    public override void OnStopInteractingPrimary() {
        base.OnStartInteractingPrimary();
        
        crop_water.Stop();
        crop_harvest.Stop();
        
        this.InteractionTimePrimary = 1;
    }

    public override bool IsInteractableSecondary() {
        // fertilize
        if (soil != null && !soil.fertilized && currentFertilizer.Value > 0) return true;

        return false;
    }

    public override void InteractSecondary() {
        base.InteractSecondary();
        
        // fertilize
        if (soil != null && !soil.fertilized && currentFertilizer.Value > 0) {
            currentFertilizer.ApplyChange(-1);
            soil.Fertilize();
            return;
        }
    }
    public override void OnStartInteractingSecondary() {
        base.OnStartInteractingSecondary();
        
        // fertilize
        if (soil != null && !soil.fertilized && currentFertilizer.Value > 0) {
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
        
        textureManager.UpdateTextures(stage, state, health);
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
        coverManager.UpdateCover(stage);
    }

    private void StartGrowing() {
        state = State.Growing;

        growthTime = (soil && soil.fertilized) ? farmingConstants.fertilizedGrowthTime : farmingConstants.baseGrowthTime;
        growthTimer = growthTime;
        
        mapIcon.UpdateMapIcon(stage, state, health);
    }

    private void StartThirsty() {
        state = State.NeedsWater;
        
        thirstyTime = (soil && soil.fertilized) ? farmingConstants.fertilizedThirstyTime : farmingConstants.baseThirstyTime;
        thirstyTimer = thirstyTime;
        
        mapIcon.UpdateMapIcon(stage, state, health);
    }

    private void Harvest() {
        ChangeCropStage(GrowthStage.Unripe);
        
        // calculate number of tomatoes to produce
        int minTomatoes = 0;
        int maxTomatoes = 0;
        // set min/max tomatoes based on health
        if (health == Health.Great) {
            minTomatoes = 2;
            maxTomatoes = 3;
        }
        else if (health == Health.Fair) {
            minTomatoes = 1;
            maxTomatoes = 2;
        }
        else if (health == Health.Wilted) {
            minTomatoes = 1;
            maxTomatoes = 1;
        }
        var tomatoesYielded = Random.Range(minTomatoes, maxTomatoes);
        playerTomatoes.ApplyChange(tomatoesYielded);
        
        // callback
        if (TomatoNotification.Instance) TomatoNotification.Instance.tomatoes = tomatoesYielded;
        onHarvest.Raise();
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
        coverManager.UpdateCover(stage);
        mapIcon.UpdateMapIcon(stage, state, health);
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

    public override string GetButtonPromptPrimary() {
        if (stage == GrowthStage.Bare || health == Health.Dead) {
            return GetInteractPrimaryButton() + " dig up";
        }
        else if (state == State.NeedsWater) {
            return currentWater.Value <= 0 ? "out of water" : GetInteractPrimaryButton() + " water";
        }
        else if (stage == GrowthStage.Ripe) {
            return GetInteractPrimaryButton() + " harvest tomato";
        }
        return "";
    }

    public override string GetButtonPromptSecondary() {
        if (soil != null && !soil.fertilized && currentFertilizer.Value > 0) {
            return GetInteractSecondaryButton() + " fertilize";
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
