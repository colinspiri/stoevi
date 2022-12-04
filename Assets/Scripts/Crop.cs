using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Crop : Interactable {
    // components
    public SpriteRenderer spriteRenderer;
    public Sprite waterSprite;
    public Sprite harvestSprite;
    public Sprite emptySprite;
    
    // constants
    public int maxTomatoes;
    public int minTomatoes;
    public float ripenTime;

    // state
    public enum CropStage { Seed, Intermediate, Unripe, Ripening, Ripe, Bare }
    public CropStage stage;

    private int tomatoesLeft;
    private float ripenTimer;

    private bool needsWater;
    private bool wateredToday;
    private int timesMissedWatering;

    protected override void Start() {
        base.Start();
        ChangeCropStage(stage);
        tomatoesLeft = Random.Range(minTomatoes, maxTomatoes + 1);
    }

    public override void Interact() {
        base.Interact();
        
        if (needsWater && !GameManager.Instance.IsWaterEmpty() && (stage == CropStage.Seed || stage == CropStage.Intermediate || stage == CropStage.Unripe)) Water();
        else if (stage == CropStage.Ripe) Harvest();
        
        InteractableManager.Instance.CheckForHarvestableCropsLeft();
    }

    private void Water() {
        GameManager.Instance.UseWater();
        AudioManager.Instance.PlayWaterSound();

        needsWater = false;
        wateredToday = true;
        if (stage == CropStage.Unripe) {
            StartCoroutine(Ripen());
        }
    }

    private void Harvest() {
        GameManager.Instance.PlayerHarvestedTomato();
        AudioManager.Instance.PlayHarvestSound();
        RemoveRipeTomatoes();
    }

    public void RemoveRipeTomatoes() {
        tomatoesLeft--;
        needsWater = true;
        ChangeCropStage(tomatoesLeft <= 0 ? CropStage.Bare : CropStage.Unripe);
    }

    private void AdvanceToNextDay() {
        if (!wateredToday) timesMissedWatering++;
        if (timesMissedWatering == 1) {
            // TODO: crop sprite is "unhealthy" version
        }
        if (timesMissedWatering >= 2) {
            // TODO: crop dies
        }

        ChangeCropStage(stage switch {
            CropStage.Seed => CropStage.Intermediate,
            CropStage.Intermediate => CropStage.Unripe,
            CropStage.Unripe => CropStage.Ripe,
            _ => stage
        });
    }

    private IEnumerator Ripen() {
        ChangeCropStage(CropStage.Ripening);
        
        // count down ripen timer
        ripenTimer = ripenTime;
        while (ripenTimer > 0) {
            ripenTimer -= Time.deltaTime;
            yield return null;
        }
        
        ChangeCropStage(CropStage.Ripe);
    }

    private void ChangeCropStage(CropStage newStage) {
        stage = newStage;
        
        // change sprite
        spriteRenderer.sprite = stage switch {
            CropStage.Seed => waterSprite,
            CropStage.Intermediate => waterSprite,
            CropStage.Unripe => waterSprite,
            CropStage.Ripening => waterSprite,
            CropStage.Ripe => harvestSprite,
            CropStage.Bare => emptySprite,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (stage == CropStage.Seed || stage == CropStage.Intermediate || stage == CropStage.Unripe) {
            needsWater = true;
        }
        else needsWater = false;

        if (stage == CropStage.Bare) {
            SetInteractable(false);
            InteractableManager.Instance.CheckForHarvestableCropsLeft();
        }
    }

    public override string GetUIText() {
        string uiText = "crop stage: " + stage.ToString().ToLower();
        if (stage == CropStage.Unripe || stage == CropStage.Ripe) uiText += " " + tomatoesLeft;

        uiText += "\n";
        
        switch (stage) {
            case CropStage.Seed:
            case CropStage.Intermediate:
            case CropStage.Unripe: {
                if (!needsWater) uiText += "already watered";
                else uiText += GameManager.Instance.IsWaterEmpty() ? "out of water" : "E to water tomato";
                break;
            }
            case CropStage.Ripening:
                uiText += "ripe in " + Mathf.Ceil(ripenTimer).ToString("0") + "s";
                break;
            case CropStage.Ripe:
                uiText += "E to harvest tomato";
                break;
            default:
                uiText += "ERROR";
                break;
        }

        return uiText;
    }

    public float GetRipenTimeFloat() {
        return ripenTimer / ripenTime;
    }
}
