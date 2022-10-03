using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class PlayChaseStinger : Action
{

    public override TaskStatus OnUpdate()
    {
        AudioManager.Instance.PlayChaseSound();
        return TaskStatus.Success;
    }
}