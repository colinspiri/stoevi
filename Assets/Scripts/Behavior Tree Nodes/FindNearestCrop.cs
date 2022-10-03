using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class FindNearestCrop : Conditional {
    public SharedCrop targetCrop;

    public bool onlyLookWithinRadius;
    public float radius;
    
    
    public override TaskStatus OnUpdate() {
        if (onlyLookWithinRadius) {
            targetCrop.Value = InteractableManager.Instance.GetClosestHarvestableCropTo(transform.position, radius);
        }
        else {
            targetCrop.Value = InteractableManager.Instance.GetClosestHarvestableCropTo(transform.position);
        }
        
        return targetCrop.Value == null ? TaskStatus.Failure : TaskStatus.Success;
    }
}