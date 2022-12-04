using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

public class StealCrop : Action {
     public SharedCrop targetCrop;
     public float stealTime;

     private float stealTimer;
     
     public override void OnStart() {
          base.OnStart();
          stealTimer = 0;
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
          GameManager.Instance.TorbalanStoleTomato();
          targetCrop.Value.RemoveRipeTomatoes();
          
          return TaskStatus.Success;
     }
}