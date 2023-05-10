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
    public enum InteractionState { NeedsWater, Growing, Harvestable, DigUp }
    private InteractionState interactionState;
    public enum Health { Fair, Wilted, Dead }
    public Health health;
    
    // growth
    private float growthTime;
    private float growthTimer;
    
    // wilting
    private bool wateredToday;
    private bool isRaining;

    private void Awake() {
        textureManager = GetComponent<CropTextureManager>();
    }

    protected override void Start() {
        base.Start();
        
        wateredToday = false;
        isRaining = false;

        int day = PlayerPrefs.GetInt("CurrentDay", 1);
        if (day >= 4) {
            wateredToday = true;
            isRaining = true;
            health = health switch {
                Health.Fair => Health.Wilted,
                Health.Wilted => Health.Dead,
                _ => health,
            };
        }
        
        ChangeCropStage(stage);
    }
    
    private void Update() {
        // growth timer
        if (interactionState == InteractionState.Growing) {
            growthTimer -= Time.deltaTime;
            if(growthTimer <= 0) Grow();
                
            // update map icon
            mapIcon.UpdateTimer(1 - (growthTimer / growthTime));
        }
    }

    private void Wilt() {
        if (health == Health.Fair) {
            health = Health.Wilted;
            
            textureManager.UpdateTextures(stage, interactionState, health, soil.fertilized);
            coverManager.UpdateCover(stage);
        }
        else if (health == Health.Wilted) {
            health = Health.Dead;
            
            textureManager.UpdateTextures(stage, interactionState, health, soil.fertilized);
            coverManager.UpdateCover(stage);
        }
    }


    public override bool IsInteractablePrimary() {
        // can be removed
        if (interactionState == InteractionState.DigUp) return true;
        /*
        if (stage == GrowthStage.Bare || health == Health.Dead) return true;
        */
        
        // can be watered
        if (interactionState == InteractionState.NeedsWater && currentWater.Value > 0) {
            return true;
        }

        // can be harvested
        if (interactionState == InteractionState.Harvestable) return true;

        // DEBUG: shorten growth timer
        #if true && UNITY_EDITOR
        if (interactionState == InteractionState.Growing) {
            return true;
        }
        #endif

        return false;
    }

    public override void InteractPrimary() {
        // can be removed
        if (interactionState == InteractionState.DigUp) {
            DigUp();
            return;
        }
        // can be harvested
        if (interactionState == InteractionState.Harvestable) {
            Harvest();
            return;
        }
        // can be watered
        if (interactionState == InteractionState.NeedsWater && currentWater.Value > 0) {
            Water();
            return;
        }
        // DEBUG: shorten growth timer
        #if true && UNITY_EDITOR
        if (interactionState == InteractionState.Growing) {
            growthTimer = 0.1f;
            return;
        }
        #endif
    }

    public override void OnStartInteractingPrimary() {
        base.OnStartInteractingPrimary();
        
        // if starting to water
        if (interactionState == InteractionState.NeedsWater && currentWater.Value > 0) {
            crop_water.Play();
            this.InteractionTimePrimary = crop_water.clip.length;
        }
        // harvest or dig up bare
        else if (interactionState == InteractionState.Harvestable || interactionState == InteractionState.DigUp) {
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
        if (stage == GrowthStage.Bare) return false;
        
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
    
    private void DigUp() {
        // can only collect seeds if not also dead
        if (health != Health.Dead) {
            seeds.ApplyChange(1);
            item_pickup.Play();
        }
        Destroy(gameObject);
    }

    public void Fertilize() {
        // objective 
        if(ObjectiveUI.Instance != null) ObjectiveUI.Instance.FinishPrompt("Fertilize");
        
        // reduce grow timer
        if (interactionState == InteractionState.Growing) {
            float reductionProportion = (farmingConstants.baseGrowthTime - farmingConstants.fertilizedGrowthTime) / farmingConstants.baseGrowthTime;
            growthTimer -= growthTimer * reductionProportion;
        }

        // increase health
        if (health == Health.Wilted) health = Health.Fair;
        
        UpdateComponents();
    }

    private void Water() {
        if(ObjectiveUI.Instance != null) ObjectiveUI.Instance.FinishPrompt("Water");

        currentWater.ApplyChange(-1);

        wateredToday = true;

        StartGrowing();
        
        UpdateComponents();
    }

    private void StartGrowing() {
        // start growing
        if (stage == GrowthStage.Sprout || stage == GrowthStage.Intermediate ||
            stage == GrowthStage.Unripe) {
            interactionState = InteractionState.Growing;

            growthTime = (soil && soil.fertilized) ? farmingConstants.fertilizedGrowthTime : farmingConstants.baseGrowthTime;
            growthTimer = growthTime;
        }
    }

    private void Harvest() {
        // calculate number of tomatoes to produce
        int minTomatoes = 0;
        int maxTomatoes = 0;
        // set min/max tomatoes based on health
        if (health == Health.Fair && soil != null && soil.fertilized) {
            minTomatoes = 2;
            maxTomatoes = 2;
        }
        else if (health == Health.Fair) {
            minTomatoes = 1;
            maxTomatoes = 1;
        }
        else if (health == Health.Wilted) {
            minTomatoes = 1;
            maxTomatoes = 1;
        }
        var tomatoesYielded = Random.Range(minTomatoes, maxTomatoes);
        playerTomatoes.ApplyChange(tomatoesYielded);
        
        ChangeCropStage(GrowthStage.Unripe);
        
        // callback
        if (TomatoNotification.Instance) TomatoNotification.Instance.tomatoes = tomatoesYielded;
        onHarvest.Raise();
    }

    public void Destroy() {
        ChangeCropStage(GrowthStage.Bare);
    }
    
    public void AdvanceToNextDay() {
        if (!wateredToday) {
            Wilt();
        }
    }

    private void Grow() {
        ChangeCropStage(stage switch {
            GrowthStage.Sprout => GrowthStage.Intermediate,
            GrowthStage.Intermediate => GrowthStage.Ripe,
            GrowthStage.Unripe => GrowthStage.Ripe,
        });
    }

    private void ChangeCropStage(GrowthStage newStage) {
        stage = newStage;

        // set interaction state based on new stage
        switch (stage) {
            case GrowthStage.Sprout:
            case GrowthStage.Intermediate:
            case GrowthStage.Unripe:
                if (isRaining == false) {
                    interactionState = InteractionState.NeedsWater;
                }
                else {
                    StartGrowing();
                }
                break;
            case GrowthStage.Ripe:
                interactionState = InteractionState.Harvestable;
                if(ObjectiveUI.Instance != null) ObjectiveUI.Instance.FinishPrompt("Ripe");
                break;
            case GrowthStage.Bare:
                interactionState = InteractionState.DigUp;
                break;
        }

        if (health == Health.Dead) {
            interactionState = InteractionState.DigUp;
        }
        
        // update components
        UpdateComponents();
    }

    private void UpdateComponents() {
        textureManager.UpdateTextures(stage, interactionState, health, soil.fertilized);
        coverManager.UpdateCover(stage);
        mapIcon.UpdateMapIcon(stage, interactionState, health);
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
            if (isRaining) {
                int day = PlayerPrefs.GetInt("CurrentDay", 1);
                if (day == 5) return "bleeding";
                else if (day == 4) return "overwatered";
            }
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
        else if (interactionState == InteractionState.NeedsWater) {
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
        if(interactionState == InteractionState.Growing) return 1 - (growthTimer / growthTime);
        return 0;
    }
    public override float GetTimerTime() {
        if (interactionState == InteractionState.Growing) return growthTimer;
        return 0;
    }
    public override InteractableUI.TimerIcon GetTimerIcon() {
        if (interactionState == InteractionState.Growing) {
            if (stage == GrowthStage.Unripe) return InteractableUI.TimerIcon.Ripe;
            else return InteractableUI.TimerIcon.Growth;
        }
        if (interactionState == InteractionState.NeedsWater) return InteractableUI.TimerIcon.Water;
        return InteractableUI.TimerIcon.None;
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        
        soil.RemoveCrop(this);
    }
}
