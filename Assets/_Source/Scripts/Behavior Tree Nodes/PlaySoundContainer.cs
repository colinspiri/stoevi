using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using SpookuleleAudio;

public class PlaySoundContainer : Action {
    public ASoundContainer soundContainer;
    public SharedBool play3D;

    public override TaskStatus OnUpdate()
    {
        if (play3D.Value) {
            soundContainer.Play3D(transform);
        }
        else {
            soundContainer.Play();
        }
        return TaskStatus.Success;
    }
}