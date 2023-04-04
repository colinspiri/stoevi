using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Movement {

    public class MoveRandomlyWithinRadius : NavMeshMovement {

        public SharedVector3 center;
        public SharedFloat radius;
        public SharedBool moveForever;
        public SharedFloat maximumTime;

        public SharedFloat minPauseDuration;
        public SharedFloat maxPauseDuration;
        
        private enum State { Moving, Paused }
        private State currentState;

        private float exitTimer; // counts up
        private float pauseTimer; // counts down

        public override void OnStart() {
            base.OnStart();

            if (maxPauseDuration.Value > 0) StartPause();
            else StartMoving();
        }


        public override TaskStatus OnUpdate() {
            if (moveForever.Value == false) {
                exitTimer += Time.deltaTime;
                if (exitTimer >= maximumTime.Value) return TaskStatus.Success;
            }

            if (currentState == State.Moving) {
                if (HasArrived()) {
                    if (maxPauseDuration.Value > 0) StartPause();
                    else StartMoving();
                }
            }

            if (currentState == State.Paused) {
                pauseTimer -= Time.deltaTime;
                if (pauseTimer <= 0) {
                    StartMoving();
                }
            }
            
            return TaskStatus.Running;
        }

        private void StartMoving() {
            currentState = State.Moving;

            Vector3 randomPoint = Util.RandomPointInRadius(center.Value, radius.Value);
            SetDestination(randomPoint);
        }

        private void StartPause() {
            currentState = State.Paused;

            pauseTimer = Random.Range(minPauseDuration.Value, maxPauseDuration.Value);
        }
    }
}