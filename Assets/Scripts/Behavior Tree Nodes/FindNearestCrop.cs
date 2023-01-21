using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class FindNearestCrop : Conditional {
    public SharedCrop targetCrop;

    public bool withinRadiusFromSelf;
    public SharedFloat radiusFromSelf;
    [Space] 
    public bool withinRadiusFromPlayer;
    public SharedFloat radiusFromPlayer;
    [Space] 
    public bool withinRadiusFromPoint;
    public SharedVector3 point;
    public SharedFloat radiusFromPoint;
    
    public override TaskStatus OnUpdate() {
        // if no requirements on radius
        if (!withinRadiusFromSelf && !withinRadiusFromPlayer && !withinRadiusFromPoint) {
            targetCrop.Value = InteractableManager.Instance.GetClosestHarvestableCropTo(transform.position);
            return targetCrop.Value == null ? TaskStatus.Failure : TaskStatus.Success;
        }
        
        // search through all crops to find best one
        List<Crop> crops = InteractableManager.Instance.GetAllCrops();
        Crop nearestValidCrop = null;
        float nearestDistance = float.MaxValue;
        foreach (var candidate in crops) {
            if (candidate == null) continue;
            if (candidate.stage != Crop.GrowthStage.Ripe) continue;
            
            // calculate distance
            var candidateDistance = Vector3.Distance(candidate.transform.position, transform.position);
            
            // check if within radius from self
            if (withinRadiusFromSelf) {
                if (candidateDistance > radiusFromSelf.Value) continue;
            }
            
            // check if within radius from player
            if (withinRadiusFromPlayer) {
                var distance = Vector3.Distance(candidate.transform.position,
                    FirstPersonMovement.Instance.transform.position);
                if (distance > radiusFromPlayer.Value) continue;
            }
            
            // check if within radius from point
            if (withinRadiusFromPoint) {
                var distance = Vector3.Distance(candidate.transform.position, point.Value);
                if (distance > radiusFromPoint.Value) continue;
            }

            // compare to nearest valid crop
            if (candidateDistance < nearestDistance) {
                nearestDistance = candidateDistance;
                nearestValidCrop = candidate;
            }
        }
        
        targetCrop.Value = nearestValidCrop;
        
        return targetCrop.Value == null ? TaskStatus.Failure : TaskStatus.Success;
    }
}