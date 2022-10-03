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

    // public constants
    public bool deaf;
    public float heardTime;

    // state
    private float heardTimer;
    
    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (heardTimer > 0) {
            heardTimer -= Time.deltaTime;
        }

        NearestCrop = InteractableManager.Instance.GetClosestHarvestableCropTo(transform.position);
        if(NearestCrop != null) NearestCropPosition = NearestCrop.transform.position;
        else NearestCropPosition = Vector3.zero;
    }

    public void ReportSound(Vector3 soundOrigin, float loudness) {
        if (deaf) return;
        
        var path = new NavMeshPath();
        bool pathFound = NavMesh.CalculatePath(transform.position, soundOrigin, NavMesh.AllAreas, path);
        if (!pathFound) return;
        var length = GetPathLength(path);

        if (length <= loudness) {
            heardTimer = heardTime;
            // behaviorTree.SetVariableValue("LastKnownPosition", soundOrigin);
            LastKnownPosition = soundOrigin;
        }
    }

    public bool HeardPlayer() {
        if (deaf) return false;
        return heardTimer > 0;
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
    
    // private void OnSceneGUI() {
    //     Handles.color = Color.white;
    //     Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, viewRadius);
    //     Vector3 viewAngleA = DirectionFromAngle(-viewAngle / 2, false);
    //     Vector3 viewAngleB = DirectionFromAngle(viewAngle / 2, false);
    //     
    //     Handles.DrawLine(transform.position, transform.position + viewAngleA * viewRadius);
    //     Handles.DrawLine(transform.position, transform.position + viewAngleB * viewRadius);
    //
    //     Handles.color = Color.red;
    //     if (CanSeePlayer()) {
    //         Handles.DrawLine(transform.position, PlayerController.Instance.transform.position);
    //     }
    // }
}
