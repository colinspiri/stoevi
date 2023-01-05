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
    public SharedVector3 lastKnownPosition;
    public SharedInt searchPositionsToGenerate;
    public SharedFloat searchRadius;
    
    // state
    private List<Vector3> searchPositions;

    public override void OnStart() {
        base.OnStart();
        
        GenerateSearchPositions();
    }

    public override TaskStatus OnUpdate() {
        if (searchPositions.Count == 0) return TaskStatus.Success;

        // set current destination
        SetDestination(searchPositions[0]);
        Debug.Log("moving to " + searchPositions[0]);
        
        // if arrived, pop from front
        if (HasArrived()) {
            Debug.Log("arrived");
            searchPositions.RemoveAt(0);
        }

        return TaskStatus.Running;
    }

    private void GenerateSearchPositions() {
        // initialize search positions
        if(searchPositions != null) searchPositions.Clear();
        else searchPositions = new List<Vector3>();

        // generate random points
        for (int i = 0; i < searchPositionsToGenerate.Value; i++) {
            Vector3 searchPos = Util.RandomPointInRadius(lastKnownPosition.Value, searchRadius.Value);
            searchPositions.Add(searchPos);
            Debug.Log("generated search pos " + searchPos);
        }
    }
}
