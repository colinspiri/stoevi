using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class KillPlayer : Action
{

    public override TaskStatus OnUpdate()
    {
        PauseMenuManager.Instance.GameOver(false);
        return TaskStatus.Success;
    }
}