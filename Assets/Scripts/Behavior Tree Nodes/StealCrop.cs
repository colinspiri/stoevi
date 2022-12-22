using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class StealCrop : Action {
     public SharedCrop targetCrop;
     public float stealTime;
     public Animator animator;

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
          ResourceManager.Instance.TorbalanStoleTomato();
          targetCrop.Value.RemoveRipeTomatoes();
          
          return TaskStatus.Success;
     }

     public override void OnEnd() {
          base.OnEnd();
          animator.SetBool("pickingfruit", false);
     }
}