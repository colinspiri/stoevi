using BehaviorDesigner.Runtime.Tasks;

public class PlayChaseStinger : Action
{

    public override TaskStatus OnUpdate()
    {
        AudioManager.Instance.PlayChaseStinger();
        return TaskStatus.Success;
    }
}