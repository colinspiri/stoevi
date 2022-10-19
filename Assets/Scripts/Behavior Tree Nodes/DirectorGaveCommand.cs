using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class DirectorGaveCommand : Conditional {
    public TorbalanDirector director;
    
    public override TaskStatus OnUpdate() {
        return director.GaveCommand() ? TaskStatus.Success : TaskStatus.Failure;
    }
}