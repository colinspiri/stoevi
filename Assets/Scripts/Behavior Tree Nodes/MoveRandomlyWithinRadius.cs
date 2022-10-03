using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tasks.Movement {

    public class MoveRandomlyWithinRadius : NavMeshMovement {

        public SharedVector3 center;
        public SharedFloat radius;
        public SharedBool moveForever;
        public SharedFloat maximumTime;

        public SharedFloat minPauseDuration;
        public SharedFloat maxPauseDuration;

        private float timer;
        private float pauseTime;
        private float destinationReachTime;

        public override void OnStart() {
            base.OnStart();

            SetDestination(GetRandomPosition());
            timer = 0;
            destinationReachTime = -1;
        }


        public override TaskStatus OnUpdate() {
            if (moveForever.Value == false) {
                timer += Time.deltaTime;
                if (timer >= maximumTime.Value) return TaskStatus.Success;
            }

            if (HasArrived()) {
                if (maxPauseDuration.Value > 0) {
                    if (destinationReachTime == -1) {
                        destinationReachTime = Time.time;
                        pauseTime = Random.Range(minPauseDuration.Value, maxPauseDuration.Value);
                    }
                    if (destinationReachTime + pauseTime <= Time.time) {
                        SetDestination(GetRandomPosition());
                        destinationReachTime = -1;
                    }
                }
            }
            
            return TaskStatus.Running;
        }

        private Vector3 GetRandomPosition() {
            Vector2 randForward = Random.insideUnitCircle.normalized;
            float randDist = Random.value * radius.Value;
            Vector3 targetPos = center.Value + (new Vector3(randForward.x, center.Value.y, randForward.y) * randDist);
            return targetPos;
        }
    }
}