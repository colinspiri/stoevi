using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class KillPlayer : Action
{

    public override TaskStatus OnUpdate()
    {
        GameManager.Instance.GameOver(false);
        return TaskStatus.Success;
    }
}