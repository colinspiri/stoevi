using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;
using UnityEngine.AI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class TorbalanSearch : NavMeshMovement {
    // components
    public TorbalanVision vision;
    
    // constants
    public SharedVector3 lastKnownPosition;
    public SharedInt searchPositionsToGenerate;
    public SharedFloat searchRadius;
    public SharedFloat minPauseDuration;
    public SharedFloat maxPauseDuration;

    // state
    private List<Vector3> searchPositions;
    private bool visitedCenter;
    private enum State { Moving, Paused }
    private State currentState;
    private float pauseTimer;

    public override void OnStart() {
        base.OnStart();
        
        SetFast(true);
        
        GenerateSearchPositions();
        
        Owner.RegisterEvent("LastKnownPositionUpdated", GenerateSearchPositions);
    }

    public override TaskStatus OnUpdate() {
        if(visitedCenter) CheckIfPointsInVision();
        
        if (searchPositions.Count == 0) return TaskStatus.Success;
        
        // moving
        if (currentState == State.Moving) {
            // if arrived, pop from front
            if (HasArrived()) {
                searchPositions.RemoveAt(0);
                visitedCenter = true;
                // SetFast(false);
                
                StartPause();
            }
        }

        // paused
        if (currentState == State.Paused) {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0) {
                StartMoving();
            }
        }

        return TaskStatus.Running;
    }

    public override void OnEnd() {
        base.OnEnd();
        
        Owner.UnregisterEvent("LastKnownPositionUpdated", GenerateSearchPositions);
    }
    
    private void StartMoving() {
        currentState = State.Moving;
        pauseTimer = 0;

        SetDestination(searchPositions[0]);
    }

    private void StartPause() {
        currentState = State.Paused;

        pauseTimer = Random.Range(minPauseDuration.Value, maxPauseDuration.Value);
    }

    private void CheckIfPointsInVision() {
        int maxPoints = searchPositions.Count;
        for (int i = 0; i < maxPoints; i++) {
            if (vision.CanSeePointInFocusedVision(searchPositions[i])) {
                Debug.Log("can see point " + i + ", removing");
                Debug.Log("searchPositions left = " + (searchPositions.Count - 1));
                searchPositions.RemoveAt(i);
                i--;
                maxPoints--;
            }
        }
    }

    private void GenerateSearchPositions() {
        // initialize search positions
        if(searchPositions != null) searchPositions.Clear();
        else searchPositions = new List<Vector3>();
        
        // add center
        searchPositions.Add(lastKnownPosition.Value);
        visitedCenter = false;
        // SetFast(true);

        // generate random points
        List<Vector3> randomPoints = new List<Vector3>();
        for (int i = 0; i < searchPositionsToGenerate.Value; i++) {
            // generate point that is on the navmesh
            Vector3 point;
            NavMeshHit hit;
            do {
                point = Util.RandomPointInRadius(lastKnownPosition.Value, searchRadius.Value);
            } while (!NavMesh.SamplePosition(point, out hit, 1.0f, NavMesh.AllAreas));
            
            // add to list
            randomPoints.Add(point);
        }
        
        // sort by distance to torbalan
        randomPoints = randomPoints.OrderBy(x => Vector3.Distance(x, transform.position)).ToList();

        // if not visible, search those points first
        int maxPoints = randomPoints.Count;
        for (int i = 0; i < maxPoints; i++) {
            if (!vision.CanSeePointInAnyVision(randomPoints[i])) {
                searchPositions.Add(randomPoints[i]);
                randomPoints.RemoveAt(i);
                i--;
                maxPoints--;
            }
        }
        
        // add the rest of the points to search positions
        foreach (var point in randomPoints) {
            searchPositions.Add(point);
        }
        
        // start moving
        StartMoving();
    }
}
