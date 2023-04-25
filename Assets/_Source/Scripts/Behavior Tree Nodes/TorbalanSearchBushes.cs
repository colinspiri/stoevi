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
    public SharedFloat pauseTime;
    public SharedFloat maxSearchTime;

    // state
    private List<Cover> bushesToSearch;
    private List<Vector3> bushPatrolPoints;
    private float plantAreaCost;
    private float pauseTimer;
    private float searchTimer;

    public override void OnStart() {
        base.OnStart();

        plantAreaCost = NavMesh.GetAreaCost(3);
        pauseTimer = 0;

        Owner.RegisterEvent("LastKnownPositionUpdated", FindBushesToSearch);
        FindBushesToSearch();
    }

    public override TaskStatus OnUpdate() {
        if (bushesToSearch.Count == 0) return TaskStatus.Success;
        
        // count timer
        searchTimer += Time.deltaTime;
        if (searchTimer >= maxSearchTime.Value) {
            return TaskStatus.Success;
        }

        // arrived
        if (HasArrived()) {
            // reset timer
            if (pauseTimer <= 0) {
                pauseTimer = pauseTime.Value;
                Stop();
                // Debug.Log("reset timer");
            }
            
            // count down timer
            if (pauseTimer > 0) {
                pauseTimer -= Time.deltaTime;
                // Debug.Log("paused " + pauseTimer);
            
                // if timer finished, choose new destination
                if(pauseTimer <= 0) {
                    // was patrolling
                    if (bushPatrolPoints.Count > 0) {
                        bushPatrolPoints.RemoveAt(0);
                
                        // more points to patrol
                        if (bushPatrolPoints.Count > 0) {
                            // Debug.Log("patrolling next point");
                            SetDestination(bushPatrolPoints[0]);
                            return TaskStatus.Running;
                        }
                        // move into bush
                        else {
                            // Debug.Log("moving into bush");
                            Cover currentBush = bushesToSearch[0];
                            SetDestination(GetClosestPointOnNavMesh(currentBush.transform.position));
                            // set bush cost to normal
                            NavMesh.SetAreaCost(3, 1);
                            return TaskStatus.Running;
                        }
                    }
                    // was moving into bush
                    else {
                        bushesToSearch.RemoveAt(0);
                        // set bush cost back to normal
                        NavMesh.SetAreaCost(3, plantAreaCost);
                
                        // more bushes to check
                        if (bushesToSearch.Count > 0) {
                            // Debug.Log("start next bush");
                            StartPatrollingBush();
                            return TaskStatus.Running;
                        }
                        // finished all bushes
                        else {
                            // Debug.Log("finished all bushes");
                            return TaskStatus.Success;
                        }
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
        Vector3 corner1 = GetClosestPointOnNavMesh(bounds.min);
        corner1.y = bounds.center.y;
        Vector3 corner3 = GetClosestPointOnNavMesh(bounds.max);
        corner3.y = bounds.center.y;
        Vector3 corner2 = GetClosestPointOnNavMesh(new Vector3(corner1.x, bounds.center.y, corner3.z));
        Vector3 corner4 = GetClosestPointOnNavMesh(new Vector3(corner3.x, bounds.center.y, corner1.z));
        
        bushPatrolPoints.Add(corner1);
        bushPatrolPoints.Add(corner2);
        bushPatrolPoints.Add(corner3);
        bushPatrolPoints.Add(corner4);

        SetDestination(bushPatrolPoints[0]);
    }

    private Vector3 GetClosestPointOnNavMesh(Vector3 inPoint) {
        Vector3 point;
        NavMeshHit hit;
        float radius = 0;
        do {
            Vector2 randomDirection2D = Random.insideUnitCircle.normalized;
            Vector3 randomDirection = new Vector3(randomDirection2D.x, 0, randomDirection2D.y);

            point = inPoint + randomDirection * radius;
            radius += 0.1f;
        } while(!NavMesh.SamplePosition(point, out hit, 10.0f, NavMesh.AllAreas));

        return hit.position;
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
        
        NavMesh.SetAreaCost(3, plantAreaCost);
        
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
