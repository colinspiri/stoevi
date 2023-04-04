using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class StealCrop : Action {
     public SharedCrop targetCrop;
     public float stealTime;
     public Animator animator;
     public IntVariable torbalanTomatoes;

     private float stealTimer;
     
     public override void OnStart() {
          base.OnStart();
          stealTimer = 0;
          animator.SetBool("pickingfruit", true);
     }
     
     public override TaskStatus OnUpdate() {
          if (targetCrop == null) {
               return TaskStatus.Failure;
          }
          
          // wait for timer
          stealTimer += Time.deltaTime;
          if (stealTimer < stealTime) {
               return TaskStatus.Running;
          }
          
          // steal crop
          torbalanTomatoes.ApplyChange(1);
          
          targetCrop.Value.Destroy();
          
          return TaskStatus.Success;
     }

     public override void OnEnd() {
          base.OnEnd();
          animator.SetBool("pickingfruit", false);
     }
}