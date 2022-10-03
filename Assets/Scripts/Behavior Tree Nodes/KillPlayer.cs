using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class KillPlayer : Action
{

    public override TaskStatus OnUpdate()
    {
        MenuManager.Instance.GameOver(false);
        return TaskStatus.Success;
    }
}