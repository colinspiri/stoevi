using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropTextureManager : MonoBehaviour {
    public FarmingConstants farmingConstants;
    public List<MeshRenderer> meshRenderers;
    public ParticleSystem particles;

    public void UpdateTextures(Crop.GrowthStage stage, Crop.State state, Crop.Health health) {
        Material newMaterial = null;

        if (stage == Crop.GrowthStage.Sprout) {
            if (health == Crop.Health.Fair || health == Crop.Health.Great) {
                if (state == Crop.State.NeedsWater) newMaterial = farmingConstants.sproutHealthyWater;
                else if (state == Crop.State.Growing) newMaterial = farmingConstants.sproutHealthyGrowing;
            }
            else if (health == Crop.Health.Wilted) {
                if (state == Crop.State.NeedsWater) newMaterial = farmingConstants.sproutWiltedWater;
                else if (state == Crop.State.Growing) newMaterial = farmingConstants.sproutWiltedGrowing;
            }
            else if (health == Crop.Health.Dead) {
                Debug.LogError(stage + " has no sprite for Dead");
            }
        }
        else if (stage == Crop.GrowthStage.Intermediate) {
            if (health == Crop.Health.Fair || health == Crop.Health.Great) {
                if (state == Crop.State.NeedsWater) newMaterial = farmingConstants.intermediateHealthyWater;
                else if (state == Crop.State.Growing) newMaterial = farmingConstants.intermediateHealthyGrowing;
            }
            else if (health == Crop.Health.Wilted) {
                if (state == Crop.State.NeedsWater) newMaterial = farmingConstants.intermediateWiltedWater;
                else if (state == Crop.State.Growing) newMaterial = farmingConstants.intermediateWiltedGrowing;
            }
            else if (health == Crop.Health.Dead) {
                newMaterial = farmingConstants.intermediateDead;
            }
        }
        else if (stage == Crop.GrowthStage.Unripe) {
            if (health == Crop.Health.Fair || health == Crop.Health.Great) {
                if (state == Crop.State.NeedsWater) newMaterial = farmingConstants.unripeHealthyWater;
                else if (state == Crop.State.Growing) newMaterial = farmingConstants.unripeHealthyGrowing;
            }
            else if (health == Crop.Health.Wilted) {
                if (state == Crop.State.NeedsWater) newMaterial = farmingConstants.unripeWiltedWater;
                else if (state == Crop.State.Growing) newMaterial = farmingConstants.unripeWiltedGrowing;
            }
            else if (health == Crop.Health.Dead) {
                newMaterial = farmingConstants.unripeDead;
            }
        }
        else if (stage == Crop.GrowthStage.Ripe) {
            if (health == Crop.Health.Fair || health == Crop.Health.Great) {
                newMaterial = farmingConstants.ripeHealthy;
            }
            else if (health == Crop.Health.Wilted) {
                newMaterial = farmingConstants.ripeWilted;
            }
            else if (health == Crop.Health.Dead) {
                Debug.LogError(stage + " has no sprite for Dead");
            }
        }
        else if (stage == Crop.GrowthStage.Bare) {
            if (health == Crop.Health.Fair || health == Crop.Health.Great) {
                newMaterial = farmingConstants.bareHealthy;
            }
            else if (health == Crop.Health.Wilted) {
                newMaterial = farmingConstants.bareWilted;
            }
            else if (health == Crop.Health.Dead) {
                Debug.LogError(stage + " has no sprite for Dead");
            }
        }

        /*if (health == Crop.Health.Dead) {
            newMaterial = farmingConstants.emptyMaterial;
        }
        else {
            switch (stage) {
                case Crop.GrowthStage.Seed:
                case Crop.GrowthStage.Sprout:
                case Crop.GrowthStage.Intermediate:
                case Crop.GrowthStage.Unripe:
                    if (state == Crop.State.NeedsWater) newMaterial = farmingConstants.emptyMaterial;
                    else if (state == Crop.State.Growing) newMaterial = farmingConstants.waterMaterial;
                    break;
                case Crop.GrowthStage.Ripe:
                    newMaterial = farmingConstants.harvestMaterial;
                    break;
                case Crop.GrowthStage.Bare:
                    newMaterial = farmingConstants.emptyMaterial;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }*/

        // set all renderers
        foreach (var meshRenderer in meshRenderers) {
            meshRenderer.material = newMaterial;
        }
        
        // set particle system based on health
        if (health == Crop.Health.Great) {
            particles.Play();
        }
        else particles.Stop();
        
    }
}
