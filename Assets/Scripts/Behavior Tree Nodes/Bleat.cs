using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SpookuleleAudio;

public class Bleat : Action {
    public ASoundContainer bleat;

    public override TaskStatus OnUpdate()
    {
        bleat.Play3D(transform);
        return TaskStatus.Success;
    }
}