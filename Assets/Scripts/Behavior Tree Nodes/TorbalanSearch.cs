using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class TorbalanSearch : NavMeshMovement {
    // constants
    public TorbalanVision vision;
    public SharedVector3 lastKnownPosition;
    public SharedInt searchPositionsToGenerate;
    public SharedFloat searchRadius;

    // state
    private List<Vector3> searchPositions;

    public override void OnStart() {
        base.OnStart();
        
        GenerateSearchPositions();
        
        Owner.RegisterEvent("LastKnownPositionUpdated", LastKnownPositionUpdated);
    }

    public override TaskStatus OnUpdate() {
        CheckIfPointsInVision();
        
        if (searchPositions.Count == 0) return TaskStatus.Success;

        // set current destination
        SetDestination(searchPositions[0]);
        
        // if arrived, pop from front
        if (HasArrived()) {
            Debug.Log("arrived");
            Debug.Log("searchPositions left = " + searchPositions.Count);
            searchPositions.RemoveAt(0);
        }

        return TaskStatus.Running;
    }

    public override void OnEnd() {
        base.OnEnd();
        Owner.UnregisterEvent("LastKnownPositionUpdated", LastKnownPositionUpdated);
    }

    private void LastKnownPositionUpdated() {
        GenerateSearchPositions();
    }

    private void CheckIfPointsInVision() {
        int maxPoints = searchPositions.Count;
        for (int i = 0; i < maxPoints; i++) {
            if (vision.CanSeePoint(searchPositions[i])) {
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

        // generate random points
        for (int i = 0; i < searchPositionsToGenerate.Value; i++) {
            Vector3 searchPos = Util.RandomPointInRadius(lastKnownPosition.Value, searchRadius.Value);
            searchPositions.Add(searchPos);
            Debug.Log("generated search pos " + searchPos);
        }
    }
}
