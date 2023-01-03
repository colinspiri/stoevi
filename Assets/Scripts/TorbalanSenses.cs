using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using BehaviorDesigner.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Vector3 = UnityEngine.Vector3;

public class TorbalanSenses : MonoBehaviour {
    public static TorbalanSenses Instance;
    
    // synced with behavior tree
    public Vector3 LastKnownPosition { get; set; }
    public Crop NearestCrop { get; set; }
    public Vector3 NearestCropPosition { get; set; }
    public float Awareness { get; set; }

    // HEARING
    // constants
    [Header("Hearing")]
    public bool deaf;
    public float heardTime;
    // state
    private float heardTimer;
    
    // VISION
    // constants
    [Header("Vision")]
    public bool blind;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public Vector3 eyesOffset;
    public Vector3 targetOffset;
    public float viewRadius;
    [Range(0, 360)] public float viewAngle;
    public float baseAwarenessSpeed;
    // state
    private bool playerWithinSight;
    

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (heardTimer > 0) {
            heardTimer -= Time.deltaTime;
        }

        LookForPlayer();

        if (playerWithinSight) {
            Awareness += baseAwarenessSpeed * Time.deltaTime;
            if (Awareness > 1) Awareness = 1;
        }
        else if (Awareness > 0) Awareness -= baseAwarenessSpeed * Time.deltaTime;
        else Awareness = 0;
    }

    public void ReportSound(Vector3 soundOrigin, float loudness) {
        if (deaf) return;
        
        var path = new NavMeshPath();
        bool pathFound = NavMesh.CalculatePath(transform.position, soundOrigin, NavMesh.AllAreas, path);
        if (!pathFound) return;
        var length = GetPathLength(path);

        if (length <= loudness) {
            heardTimer = heardTime;
            LastKnownPosition = soundOrigin;
        }
    }

    public bool HeardPlayer() {
        if (deaf) return false;
        return heardTimer > 0;
    }

    private void LookForPlayer() {
        if (blind) {
            playerWithinSight = false;
            return;
        }
        
        playerWithinSight = false;

        Vector3 eyesPosition = transform.position + eyesOffset;
        
        Collider[] targetsInViewRadius = Physics.OverlapSphere(eyesPosition, viewRadius, targetMask);
        
        // search through all targets in the radius
        foreach (var t in targetsInViewRadius) {
            Vector3 targetPosition = t.transform.position + targetOffset;
            Vector3 directionToTarget = (targetPosition - eyesPosition).normalized;
            float distanceToTarget = Vector3.Distance(eyesPosition, targetPosition);

            // check if within view angle
            if (Vector3.Angle(transform.forward, directionToTarget) > viewAngle / 2) continue;
            
            // check if obstacles between self and player
            if (Physics.Raycast(eyesPosition, directionToTarget, distanceToTarget, obstacleMask)) continue;
            
            playerWithinSight = true;
            LastKnownPosition = targetPosition;
            return;
        }
    }
    
    private float GetPathLength(NavMeshPath path) {
        if (path.status != NavMeshPathStatus.PathComplete) return 0;
        
        float length = 0;
        for (int i = 1; i < path.corners.Length; i++) {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }

        return length;
    }

    private Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal) {
        if (!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnDrawGizmos() {
        Vector3 eyesPosition = transform.position + eyesOffset;
        
        Vector3 viewAngleA = DirectionFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirectionFromAngle(viewAngle / 2, false);
        
        Gizmos.color = Color.white;
        Gizmos.DrawLine(eyesPosition, eyesPosition + viewAngleA * viewRadius);
        Gizmos.DrawLine(eyesPosition, eyesPosition + viewAngleB * viewRadius);

        Gizmos.color = Color.red;
        if (playerWithinSight) {
            Gizmos.DrawLine(eyesPosition, FirstPersonMovement.Instance.transform.position + targetOffset);
        }
    }
}
