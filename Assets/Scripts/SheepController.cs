using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class SheepController : MonoBehaviour {
    // components
    private NavMeshAgent agent;
    private SheepSpriteManager spriteManager;
    
    // public constants
    public float walkRadius;
    public float minIdleTime, maxIdleTime;
    public float cropDistance;
    public float eatTime;
    
    // state
    private enum AIState { Idle, Walking, Eating }
    private AIState state = AIState.Idle;
    // idle
    private float idleTime;
    private float idleTimer;
    // walking
    private Vector3 targetPosition;
    private Crop targetCrop;
    // eating
    private float eatingTimer;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        spriteManager = GetComponent<SheepSpriteManager>();
    }

    private void Start() {
        StartIdle();
    }

    // Update is called once per frame
    void Update() {
        if (state == AIState.Idle) {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTime) {
                WalkToRandomLocation();
            }
        }
        else if (state == AIState.Walking) {
            if(targetCrop == null || targetCrop.cropState != Crop.CropState.Harvest) StartIdle();
            agent.SetDestination(targetPosition);
            if (CloseEnoughToDestination()) {
                StartEating();
            }
        }
        else if (state == AIState.Eating) {
            if(targetCrop == null || targetCrop.cropState != Crop.CropState.Harvest) StartIdle();
            eatingTimer += Time.deltaTime;
            if (eatingTimer >= eatTime) {
                // destroy crop & switch back to idle
                targetCrop.MakeEmpty();
                StartIdle();
            }
        }

        // look for closest crop
        targetCrop = InteractableManager.Instance.GetClosestHarvestableCropTo(transform.position);
        if (targetCrop != null) {
            float distance = Vector3.Distance(transform.position, targetCrop.transform.position);
            if (distance > cropDistance) targetCrop = null;
            // if within range and not already eating, set it as next destination
            else if (state == AIState.Idle || state == AIState.Walking) {
                targetPosition = targetCrop.transform.position;
                state = AIState.Walking;
            }
        }
        
        // Debug.Log("target crop == " + (targetCrop == null ? "null" : targetCrop.name));
    }

    private void StartIdle() {
        idleTime = Random.Range(minIdleTime, maxIdleTime);
        idleTimer = 0;
        state = AIState.Idle;
        
        spriteManager.ShowIdle();
    }
    private void StartEating() {
        eatingTimer = 0;
        state = AIState.Eating;
        
        spriteManager.ShowEating();
    }

    private void WalkToRandomLocation() {
        // pick random position on navmesh
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMesh.SamplePosition(randomDirection, out var hit, walkRadius, 1);
        
        // set destination
        agent.SetDestination(hit.position);
        
        // switch state
        state = AIState.Walking;
    }

    private bool CloseEnoughToDestination(bool debug = false) {
        Vector3 toDestination = agent.destination - transform.position;
        toDestination.y = 0; // ignore vertical component
        float distance = toDestination.magnitude;
        if(debug) Debug.Log("distance to destination = " + distance + ", with to stoppingDistance = " + agent.stoppingDistance);
        return distance <= agent.stoppingDistance;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, walkRadius);
        Gizmos.DrawWireSphere(transform.position, cropDistance);
    }
}