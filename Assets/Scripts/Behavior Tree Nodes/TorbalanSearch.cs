using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;

public class TorbalanSearch : NavMeshMovement {
    public SharedVector3 lastKnownPosition;

    public override void OnStart() {
        base.OnStart();
        
        
    }

    public override TaskStatus OnUpdate() {
        SetDestination(lastKnownPosition.Value);

        return HasArrived() ? TaskStatus.Success : TaskStatus.Running;
    }
}
