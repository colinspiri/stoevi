using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
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
    public float growTime;
    public float ripenTime;

    // state
    public enum CropStage { Seed, Intermediate, Unripe, Ripe, Bare }
    public CropStage stage;
    public bool startAtRandomStage;

    private int tomatoesLeft;
    
    private bool growing;
    private float growTimer;
    
    private bool ripening;
    private float ripenTimer;

    private bool watered;
    // private bool wateredToday;
    // private int timesMissedWatering;

    protected override void Start() {
        base.Start();
        
        if (startAtRandomStage) {
            int randomStage = Random.Range(0, 4);
            // Debug.Log("randomStage = " + randomStage + " - " + ((CropStage)randomStage).ToString());
            ChangeCropStage((CropStage)randomStage);
        }
        else ChangeCropStage(stage);
        
        tomatoesLeft = Random.Range(minTomatoes, maxTomatoes + 1);
    }

    public override void Interact() {
        if (!watered && !ResourceManager.Instance.IsWaterEmpty() && (stage == CropStage.Seed || stage == CropStage.Intermediate || stage == CropStage.Unripe)) Water();
        else if (stage == CropStage.Ripe) Harvest();
        
        InteractableManager.Instance.CheckForHarvestableCropsLeft();
    }

    private void Water() {
        ResourceManager.Instance.UseWater();
        AudioManager.Instance.PlayWaterSound();

        watered = true;
        
        if (stage == CropStage.Seed || stage == CropStage.Intermediate) {
            StartCoroutine(Grow());
        }
        else if (stage == CropStage.Unripe) {
            StartCoroutine(Ripen());
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

    private IEnumerator Grow() {
        growing = true;
        
        // count down grow timer
        growTimer = growTime;
        while (growTimer > 0) {
            growTimer -= Time.deltaTime;
            yield return null;
        }
        
        ChangeCropStage(stage switch {
            CropStage.Seed => CropStage.Intermediate,
            CropStage.Intermediate => CropStage.Unripe,
            CropStage.Unripe => CropStage.Ripe,
            _ => stage
        });

        growing = false;
        watered = false;
    }

    private IEnumerator Ripen() {
        ripening = true;
        
        // count down ripen timer
        ripenTimer = ripenTime;
        while (ripenTimer > 0) {
            ripenTimer -= Time.deltaTime;
            yield return null;
        }
        
        ChangeCropStage(CropStage.Ripe);
        ripening = false;
    }

    private void ChangeCropStage(CropStage newStage) {
        stage = newStage;
        
        // change sprite
        spriteRenderer.sprite = stage switch {
            CropStage.Seed => emptySprite,
            CropStage.Intermediate => emptySprite,
            CropStage.Unripe => waterSprite,
            CropStage.Ripe => harvestSprite,
            CropStage.Bare => emptySprite,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (stage == CropStage.Bare) {
            SetInteractable(false);
            InteractableManager.Instance.CheckForHarvestableCropsLeft();
        }
    }

    public override string GetUIText() {
        string uiText = stage.ToString().ToLower();

        uiText += "\n";
        
        switch (stage) {
            case CropStage.Seed:
            case CropStage.Intermediate:
            case CropStage.Unripe: {
                if (ripening) uiText += "ripe in " + Mathf.Ceil(ripenTimer).ToString("0") + "s";
                else if (growing) uiText += "growing in " + Mathf.Ceil(growTimer).ToString("0") + "s";
                else if (watered) uiText += "already watered";
                else uiText += ResourceManager.Instance.IsWaterEmpty() ? "out of water" : "E to water tomato";
                break;
            }
            case CropStage.Ripe:
                uiText += "E to harvest tomato";
                break;
            default:
                uiText += "ERROR";
                break;
        }

        return uiText;
    }

    public override float GetSliderFloat() {
        if (ripening) {
            return ripenTimer / ripenTime;
        }
        if (growing) {
            return growTimer / growTime;
        }
        return 0;
    }
}
