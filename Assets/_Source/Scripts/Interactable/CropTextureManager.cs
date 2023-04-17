using System.Collections.Generic;
using UnityEngine;

public class CropTextureManager : MonoBehaviour {
    public FarmingConstants farmingConstants;
    public List<MeshRenderer> meshRenderers;
    public ParticleSystem particles;

    public void UpdateTextures(Crop.GrowthStage stage, Crop.InteractionState interactionState, Crop.Health health, bool fertilized) {
        Material newMaterial = null;

        if (stage == Crop.GrowthStage.Sprout) {
            if (health == Crop.Health.Fair) {
                if (interactionState == Crop.InteractionState.NeedsWater) newMaterial = farmingConstants.sproutHealthyWater;
                else if (interactionState == Crop.InteractionState.Growing) newMaterial = farmingConstants.sproutHealthyGrowing;
            }
            else if (health == Crop.Health.Wilted) {
                if (interactionState == Crop.InteractionState.NeedsWater) newMaterial = farmingConstants.sproutWiltedWater;
                else if (interactionState == Crop.InteractionState.Growing) newMaterial = farmingConstants.sproutWiltedGrowing;
            }
            else if (health == Crop.Health.Dead) {
                newMaterial = farmingConstants.sproutDead;
            }
        }
        else if (stage == Crop.GrowthStage.Intermediate) {
            if (health == Crop.Health.Fair) {
                if (interactionState == Crop.InteractionState.NeedsWater) newMaterial = farmingConstants.intermediateHealthyWater;
                else if (interactionState == Crop.InteractionState.Growing) newMaterial = farmingConstants.intermediateHealthyGrowing;
            }
            else if (health == Crop.Health.Wilted) {
                if (interactionState == Crop.InteractionState.NeedsWater) newMaterial = farmingConstants.intermediateWiltedWater;
                else if (interactionState == Crop.InteractionState.Growing) newMaterial = farmingConstants.intermediateWiltedGrowing;
            }
            else if (health == Crop.Health.Dead) {
                newMaterial = farmingConstants.intermediateDead;
            }
        }
        else if (stage == Crop.GrowthStage.Unripe) {
            if (health == Crop.Health.Fair) {
                if (interactionState == Crop.InteractionState.NeedsWater) newMaterial = farmingConstants.unripeHealthyWater;
                else if (interactionState == Crop.InteractionState.Growing) newMaterial = farmingConstants.unripeHealthyGrowing;
            }
            else if (health == Crop.Health.Wilted) {
                if (interactionState == Crop.InteractionState.NeedsWater) newMaterial = farmingConstants.unripeWiltedWater;
                else if (interactionState == Crop.InteractionState.Growing) newMaterial = farmingConstants.unripeWiltedGrowing;
            }
            else if (health == Crop.Health.Dead) {
                newMaterial = farmingConstants.unripeDead;
            }
        }
        else if (stage == Crop.GrowthStage.Ripe) {
            if (health == Crop.Health.Fair) {
                newMaterial = farmingConstants.ripeHealthy;
            }
            else if (health == Crop.Health.Wilted) {
                newMaterial = farmingConstants.ripeWilted;
            }
            else if (health == Crop.Health.Dead) {
                newMaterial = farmingConstants.ripeDead;
            }
        }
        else if (stage == Crop.GrowthStage.Bare) {
            if (health == Crop.Health.Fair) {
                newMaterial = farmingConstants.bareHealthy;
            }
            else if (health == Crop.Health.Wilted) {
                newMaterial = farmingConstants.bareWilted;
            }
            else if (health == Crop.Health.Dead) {
                newMaterial = farmingConstants.bareDead;
            }
        }

        // set all renderers
        foreach (var meshRenderer in meshRenderers) {
            meshRenderer.material = newMaterial;
        }
        
        // set particle system based on health
        if (fertilized) {
            particles.Play();
        }
        else particles.Stop();
        
    }
}
