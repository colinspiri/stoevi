using System.Collections.Generic;
using System.Linq;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using SpookuleleAudio;
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
    public SharedFloat searchRadiusBulge;
    public SharedFloat minPauseDuration;
    public SharedFloat maxPauseDuration;
    public SharedFloat maxSearchTime;
    public BushSet bushSet;
    public SharedFloat bushSpacing;
    [Header("Huffing")] 
    public ASoundContainer torbalan_huff;
    public SharedFloat minTimeBetweenHuffs;
    public SharedFloat maxTimeBetweenHuffs;

    // state
    private List<Vector3> searchPositions;
    private bool visitedCenter;
    private enum State { Moving, Paused }
    private State currentState;
    private float pauseTimer;
    private float searchTimer;
    private float huffTimer;

    public override void OnStart() {
        base.OnStart();
        
        huffTimer = Random.Range(minTimeBetweenHuffs.Value, maxTimeBetweenHuffs.Value);

        GenerateSearchPositions();
        
        Owner.RegisterEvent("LastKnownPositionUpdated", GenerateSearchPositions);
    }

    public override TaskStatus OnUpdate() {
        CheckIfPointsInVision();
        
        if (searchPositions.Count == 0) return TaskStatus.Success;

        var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, transform.position);
        bool playerIsFar = distance > searchRadius.Value;
        SetFast(playerIsFar);
        
        // moving
        if (currentState == State.Moving) {
            // if arrived, pop from front
            if (HasArrived()) {
                searchPositions.RemoveAt(0);
                visitedCenter = true;

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
        
        // count timer
        searchTimer += Time.deltaTime;
        if (searchTimer >= maxSearchTime.Value) {
            return TaskStatus.Success;
        }
        
        // huff timer
        huffTimer -= Time.deltaTime;
        if (huffTimer <= 0) {
            torbalan_huff.Play3D(transform);
            huffTimer = Random.Range(minTimeBetweenHuffs.Value, maxTimeBetweenHuffs.Value);
        }

        return TaskStatus.Running;
    }

    public override void OnEnd() {
        base.OnEnd();
        
        searchPositions.Clear();
        Owner.UnregisterEvent("LastKnownPositionUpdated", GenerateSearchPositions);
    }
    
    private void StartMoving() {
        currentState = State.Moving;
        pauseTimer = 0;

        if(searchPositions.Count > 0) SetDestination(searchPositions[0]);
    }

    private void StartPause() {
        currentState = State.Paused;

        pauseTimer = Random.Range(minPauseDuration.Value, maxPauseDuration.Value);
    }

    private void CheckIfPointsInVision() {
        int maxPoints = searchPositions.Count;
        for (int i = 0; i < maxPoints; i++) {
            if (!visitedCenter) continue; // don't skip visiting the center
            if (vision.CanSeePoint(searchPositions[i])) {
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
        searchTimer = 0;
        
        // get forward vector to determine dot products
        Vector3 forwardVec;
        float playerFacingTorbalanDot = Vector3.Dot(FirstPersonMovement.Instance.transform.forward, transform.forward);
        // if player saw Torbalan, use Torbalan forward
        if (playerFacingTorbalanDot < 0) {
            forwardVec = transform.forward;
        }
        // if player running away, use player forward
        else {
            forwardVec = FirstPersonMovement.Instance.transform.forward;
        }

        // generate random points
        List<Vector3> randomPoints = new List<Vector3>();
        for (int i = 0; i < searchPositionsToGenerate.Value; i++) {
            // generate point that is on the navmesh
            Vector3 point;
            NavMeshHit hit;
            do {
                Vector2 randomDirection2D = Random.insideUnitCircle.normalized;
                Vector3 randomDirection = new Vector3(randomDirection2D.x, 0, randomDirection2D.y);
                float dot = -Vector3.Dot(randomDirection, forwardVec);

                // max radius is larger if in direction of player facing
                float maxRadius = searchRadius.Value;
                float bulgeDisplacement = -dot * searchRadiusBulge.Value;
                float radius = Random.Range(0, maxRadius + bulgeDisplacement);

                point = lastKnownPosition.Value + randomDirection * radius;
            } while (!NavMesh.SamplePosition(point, out hit, 1.0f, NavMesh.AllAreas));

            // add to list
            randomPoints.Add(point);
        }

        // sort by dot product
        randomPoints = randomPoints.OrderBy(point => {
            Vector3 toPoint = (point - lastKnownPosition.Value).normalized;
            float dot = -Vector3.Dot(toPoint, forwardVec);
            return dot;
        }).ToList();

        // add the rest of the points to search positions
        foreach (var point in randomPoints) {
            // discard point if inside bush
            bool insideBush = false;
            foreach (var bush in bushSet.Items) {
                Bounds bounds = bush.GetComponent<Collider>().bounds;
                bounds.Expand(bushSpacing.Value);
                if (bounds.Contains(point)) {
                    insideBush = true;
                    break;
                }
            }

            if (insideBush) continue;
            
            searchPositions.Add(point);
        }

        // start moving
        StartMoving();
    }

    public override void OnDrawGizmos() {
        base.OnDrawGizmos();

        if (!Application.isPlaying) return;

        if (searchPositions == null) return;
        
        for (int i = 0; i < searchPositions.Count; i++) {
            if(i == 0) Gizmos.color = Color.blue;
            else Gizmos.color = Color.Lerp(Color.red, Color.yellow, (float)i / searchPositions.Count);
            
            Gizmos.DrawSphere(searchPositions[i], Mathf.Lerp(1f, 0.1f, (float)i/searchPositions.Count));
        }
    }
}
