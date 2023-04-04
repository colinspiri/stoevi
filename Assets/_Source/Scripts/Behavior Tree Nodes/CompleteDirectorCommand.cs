using BehaviorDesigner.Runtime.Tasks;

public class CompleteDirectorCommand : Action {
    public TorbalanDirector director;

    public override TaskStatus OnUpdate() {
        return TaskStatus.Success;
    }
}