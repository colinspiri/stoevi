using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class EatCrop : Action {
    public SharedCrop targetCrop;
    public SheepSpriteManager spriteManager;
    public float eatTime;
    public IntVariable torbalanTomatoes;
    public AudioSource sheep_eat;

    private float eatTimer;

    public override void OnStart() {
        base.OnStart();
        
        eatTimer = 0;
        spriteManager.ShowEating();
        sheep_eat.Play();
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
        
        torbalanTomatoes.ApplyChange(1);
        
        targetCrop.Value.Destroy();
        
        return TaskStatus.Success;
    }

    public override void OnEnd() {
        base.OnEnd();
        
        spriteManager.ShowIdle();
        sheep_eat.Stop();
    }
}