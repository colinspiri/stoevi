using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class TorbalanDirector : MonoBehaviour {
    // constants
    public List<Transform> areaNodes;
    
    [Header("Backstage")]
    public float backstageDistance;
    public float maxBackstageTime;

    [Header("Frontstage")] 
    public float frontstageDistance;
    public float maxFrontstageTime;

    public float intensityRadius;
    public float maxIntensity;
    
    // behavior tree variables
    public Vector3 TargetPosition { get; set; }
    public GameObject Player { get; set; }
    public bool Frontstage { get; set; }
    public float FrontstageDistance => frontstageDistance;
    public bool Backstage { get; set; }
    public float BackstageDistance => backstageDistance;
    
    // state
    private enum DirectorState { None, Frontstage, Backstage }
    private DirectorState directorState { get; set; }
    private float intensity;
    private float frontstageTimer;
    private float backstageTimer;

    private void Start() {
        Player = FirstPersonMovement.Instance.gameObject;
        SetDirectorState(DirectorState.Backstage);
    }

    // Update is called once per frame
    void Update() {
        SetTargetPosition();

        if (directorState == DirectorState.Frontstage) {
            CountFrontstageTimer();
            CountIntensity();
        }
        else if (directorState == DirectorState.Backstage) {
            CountBackstageTimer();
        }
        else if (directorState == DirectorState.None) {
            CountIntensity();
            CheckIfBackstage();
        }
    }

    private void CountIntensity() {
        // calculate distance from player
        var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, transform.position);
        
        // if close enough, count intensity timer
        if (distance < intensityRadius) {
            intensity += Time.deltaTime;
            
            if (intensity >= maxIntensity) {
                intensity = 0;
                SetDirectorState(DirectorState.Backstage);
            }
        }
    }

    private void CountFrontstageTimer() {
        var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, transform.position);

        if (distance < frontstageDistance) {
            frontstageTimer += Time.deltaTime;
        }
        
        if (frontstageTimer >= maxFrontstageTime) {
            frontstageTimer = 0;
            SetDirectorState(DirectorState.None);
        }
    }

    private void CountBackstageTimer() {
        var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, transform.position);

        if (distance > backstageDistance) {
            backstageTimer += Time.deltaTime;
        }
        
        if (backstageTimer >= maxBackstageTime) {
            backstageTimer = 0;
            SetDirectorState(DirectorState.Frontstage);
        }
    }

    private void CheckIfBackstage() {
        // calculate distance from player
        var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, transform.position);
        
        // if already far enough, go into backstage
        if (distance > backstageDistance) {
            SetDirectorState(DirectorState.Backstage);
        }
    }

    private void SetDirectorState(DirectorState newState) {
        directorState = newState;

        Frontstage = directorState == DirectorState.Frontstage;
        Backstage = directorState == DirectorState.Backstage;
        
        SetTargetPosition();
    }
    
    private void SetTargetPosition() {
        if (directorState == DirectorState.Frontstage) {
            TargetPosition = GetClosestAreaNodeToPlayer();
        }
        else if (directorState == DirectorState.Backstage) {
            TargetPosition = GetFarthestAreaNodeFromPlayer();
        }
        else if (directorState == DirectorState.None) {
            TargetPosition = transform.position;
        }
    }

    private Vector3 GetClosestAreaNodeToPlayer() {
        float minDistance = float.MaxValue;
        Vector3 closestNode = Vector3.zero;
        foreach (var node in areaNodes) {
            var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, node.position);
            if (distance < minDistance) {
                minDistance = distance;
                closestNode = node.position;
            }
        }
        return closestNode;
    }
    
    private Vector3 GetFarthestAreaNodeFromPlayer() {
        float maxDistance = 0;
        Vector3 farthestNode = Vector3.zero;
        foreach (var node in areaNodes) {
            var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, node.position);
            if (distance > maxDistance) {
                maxDistance = distance;
                farthestNode = node.position;
            }
        }
        return farthestNode;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, intensityRadius);
        Gizmos.DrawWireSphere(transform.position, backstageDistance);
    }
}
