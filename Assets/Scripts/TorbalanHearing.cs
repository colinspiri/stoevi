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

public class TorbalanHearing : MonoBehaviour {
    public static TorbalanHearing Instance;
    
    // synced with behavior tree
    public Vector3 LastHeardLocation { get; set; }
    public bool HeardRecently { get; set; }

    // constants
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
        HeardRecently = heardTimer > 0;
    }

    public void ReportSound(Vector3 soundOrigin, float loudness) {
        if (deaf) return;
        
        var path = new NavMeshPath();
        bool pathFound = NavMesh.CalculatePath(transform.position, soundOrigin, NavMesh.AllAreas, path);
        if (!pathFound) return;
        var length = GetPathLength(path);

        if (length <= loudness) {
            heardTimer = heardTime;
            LastHeardLocation = soundOrigin;
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
}
