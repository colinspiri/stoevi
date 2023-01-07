using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class TorbalanDirector : MonoBehaviour {
    // components
    public static TorbalanDirector Instance;
    public List<Transform> areaNodes;
    public TorbalanDirectorSettings settings;

    // behavior tree variables
    public Vector3 TargetPosition { get; set; }
    public GameObject Player { get; set; }
    public bool Frontstage { get; set; }
    public float FrontstagePatrolRadius => settings.frontstagePatrolRadius;
    public bool Backstage { get; set; }
    
    // state
    private int aggressionLevel;
    private enum DirectorState { None, Frontstage, Backstage }
    private DirectorState directorState { get; set; }
    private float intensity;
    private float frontstageTimer;
    private float backstageTimer;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        Player = FirstPersonMovement.Instance.gameObject;
        SetDirectorState(DirectorState.Backstage);
        aggressionLevel = 0;
    }

    private void OnEnable() {
        DayManager.OnNight += SetAggressionMaximum;
    }
    private void OnDisable() {
        DayManager.OnNight -= SetAggressionMaximum;
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

    public void IncrementAggression() {
        aggressionLevel++;
        if (aggressionLevel > settings.maxAggressionLevel) aggressionLevel = settings.maxAggressionLevel;
        Debug.Log("aggression level increased to " + aggressionLevel);
    }
    private void SetAggressionMaximum() {
        aggressionLevel = settings.maxAggressionLevel;
        Debug.Log("aggression level maxed to " + aggressionLevel);
    }

    private void CountIntensity() {
        // calculate distance from player
        var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, transform.position);
        
        // if close enough, count intensity timer
        if (distance < settings.intensityDistance) {
            intensity += Time.deltaTime;
            Debug.Log("intensity = " + Mathf.Floor(intensity) + "/" + settings.GetMaxIntensity(aggressionLevel));
            
            if (intensity >= settings.GetMaxIntensity(aggressionLevel)) {
                intensity = 0;
                SetDirectorState(DirectorState.Backstage);
            }
        }
    }

    private void CountFrontstageTimer() {
        var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, transform.position);

        if (distance < settings.frontstageDistance) {
            frontstageTimer += Time.deltaTime;
            Debug.Log("frontstage = " + Mathf.Floor(frontstageTimer) + "/" + settings.GetFrontstageTime(aggressionLevel));
        }
        
        if (frontstageTimer >= settings.GetFrontstageTime(aggressionLevel)) {
            frontstageTimer = 0;
            SetDirectorState(DirectorState.None);
        }
    }

    private void CountBackstageTimer() {
        var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, transform.position);

        if (distance > settings.backstageDistance) {
            backstageTimer += Time.deltaTime;
            Debug.Log("backstage = " + Mathf.Floor(backstageTimer) + "/" + settings.GetBackstageTime(aggressionLevel));
        }
        
        if (backstageTimer >= settings.GetBackstageTime(aggressionLevel)) {
            backstageTimer = 0;
            SetDirectorState(DirectorState.Frontstage);
        }
    }

    private void CheckIfBackstage() {
        // calculate distance from player
        var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, transform.position);
        
        // if already far enough, go into backstage
        if (distance > settings.backstageDistance) {
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
        Gizmos.DrawWireSphere(transform.position, settings.intensityDistance);
        Gizmos.DrawWireSphere(transform.position, settings.backstageDistance);

        if (Application.isPlaying) {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(TargetPosition, 1f);
        }
    }
}
