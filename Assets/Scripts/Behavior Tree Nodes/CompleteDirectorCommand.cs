using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class CompleteDirectorCommand : Action {
    public TorbalanDirector director;

    public override TaskStatus OnUpdate() {
        director.CompleteCommand();
        return TaskStatus.Success;
    }
}