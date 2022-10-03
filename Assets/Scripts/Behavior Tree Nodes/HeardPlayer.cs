using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class HeardPlayer : Conditional {
    public TorbalanSenses senses;
    
    public override TaskStatus OnUpdate() {
        return senses.HeardPlayer() ? TaskStatus.Success : TaskStatus.Failure;
    }
}
