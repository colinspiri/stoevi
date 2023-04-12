using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;
using UnityEngine.AI;

public class TorbalanSearchBushes : NavMeshMovement {
    // components
    public BushSet bushSet;
    
    // constants
    public SharedVector3 lastKnownPosition;
    public SharedFloat searchRadius;
    public SharedFloat patrolDistanceFromBush;

    // state
    private List<Cover> bushesToSearch;
    private List<Vector3> bushPatrolPoints;

    public override void OnStart() {
        base.OnStart();
        
        Owner.RegisterEvent("LastKnownPositionUpdated", FindBushesToSearch);
        FindBushesToSearch();
    }

    public override TaskStatus OnUpdate() {
        if (bushesToSearch.Count == 0) return TaskStatus.Success;
        
        if (HasArrived()) {
            if (bushPatrolPoints != null && bushPatrolPoints.Count > 0) {
                bushPatrolPoints.RemoveAt(0);
                // more points to patrol
                if (bushPatrolPoints.Count > 0) {
                    Debug.Log("patrolling next point");
                    SetDestination(bushPatrolPoints[0]);
                }
                // onto next bush
                else {
                    bushesToSearch.RemoveAt(0);
                    // more bushes to check
                    Debug.Log("next bush");
                    if (bushesToSearch.Count > 0) {
                        StartPatrollingBush();
                    }
                    // finished all bushes
                    else {
                        Debug.Log("done");
                        return TaskStatus.Success;
                    }
                }
            }
        }
        
        return TaskStatus.Running;
    }

    private void StartPatrollingBush() {
        if (bushPatrolPoints != null) bushPatrolPoints.Clear();
        else bushPatrolPoints = new List<Vector3>();

        Cover currentBush = bushesToSearch[0];
        Collider collider = currentBush.GetComponent<Collider>();
        Bounds bounds = collider.bounds;
        bounds.Expand(patrolDistanceFromBush.Value);
        Vector3 corner1 = bounds.min;
        corner1.y = bounds.center.y;
        Vector3 corner3 = bounds.max;
        corner3.y = bounds.center.y;
        Vector3 corner2 = new Vector3(corner1.x, bounds.center.y, corner3.z);
        Vector3 corner4 = new Vector3(corner3.x, bounds.center.y, corner1.z);
        
        bushPatrolPoints.Add(corner1);
        bushPatrolPoints.Add(corner2);
        bushPatrolPoints.Add(corner3);
        bushPatrolPoints.Add(corner4);
        bushPatrolPoints.Add(bounds.center);
        bushPatrolPoints.Add(corner2);

        SetDestination(bushPatrolPoints[0]);
    }

    private void FindBushesToSearch() {
        if (bushesToSearch != null) bushesToSearch.Clear();
        else bushesToSearch = new List<Cover>();
        
        // get all bushes within the search radius
        foreach (var bush in bushSet.Items) {
            var distance = Vector3.Distance(bush.transform.position, lastKnownPosition.Value);
            if (distance <= searchRadius.Value) {
                bushesToSearch.Add(bush);
            }
        }
        
        // sort bushes by distance
        bushesToSearch = bushesToSearch.OrderBy(bush => {
            var distance = Vector3.Distance(bush.transform.position, lastKnownPosition.Value);
            return distance;
        }).ToList();
        
        // patrol first bush
        StartPatrollingBush();
    }

    public override void OnEnd() {
        base.OnEnd();
        
        Owner.UnregisterEvent("LastKnownPositionUpdated", FindBushesToSearch);
    }
    
    public override void OnDrawGizmos() {
        base.OnDrawGizmos();

        if (!Application.isPlaying) return;

        if (bushesToSearch == null) return;
        for (int i = 0; i < bushesToSearch.Count; i++) {
            if(i == 0) Gizmos.color = Color.blue;
            else Gizmos.color = Color.green;
            
            Gizmos.DrawSphere(bushesToSearch[i].transform.position, 1);
        }

        if (bushPatrolPoints == null) return;
        for (int i = 0; i < bushPatrolPoints.Count; i++) {
            if(i == 0) Gizmos.color = Color.red;
            else Gizmos.color = Color.yellow;
            
            Gizmos.DrawSphere(bushPatrolPoints[i], 1);
        }
    }
}
