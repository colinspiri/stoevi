using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EatCrop : Action {
    public SharedCrop targetCrop;
    public SheepSpriteManager spriteManager;
    public float eatTime;

    private float eatTimer;

    public override void OnStart() {
        base.OnStart();
        eatTimer = 0;
        spriteManager.ShowEating();
    }

    public override TaskStatus OnUpdate() {
        if (targetCrop == null) {
            return TaskStatus.Failure;
        }
        
        // wait for timer
        eatTimer += Time.deltaTime;
        if (eatTimer < eatTime) {
            return TaskStatus.Running;
        }
        
        TomatoCounter.Instance.TorbalanStoleTomato();
        targetCrop.Value.MakeEmpty();
        
        return TaskStatus.Success;
    }

    public override void OnEnd() {
        base.OnEnd();
        
        spriteManager.ShowIdle();
    }
}