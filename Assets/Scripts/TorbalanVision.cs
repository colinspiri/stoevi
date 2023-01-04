using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;
using BehaviorDesigner.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Vector3 = UnityEngine.Vector3;

public class TorbalanVision : MonoBehaviour {
    // components
    public Material eyesMaterial;
    
    // synced with behavior tree
    public Vector3 LastSeenPosition { get; set; }
    public float Awareness { get; set; }

    // constants
    public bool blind;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public Vector3 eyesOffset;
    public Vector3 targetOffset;
    public float viewRadius;
    [Range(0, 360)] public float viewAngle;

    [Header("Awareness")] 
    public float baseAwarenessTime;
    public float awarenessDecayTime;
    [Space]
    public float closeThreshold;
    public float farThreshold;
    public float closeFactor;
    public float mediumFactor;
    public float farFactor;
    [Space] 
    public float sideThreshold;
    public float frontFactor;
    public float sideFactor;
    [Space] 
    public float standingFactor;
    public float crouchedFactor;
    [Space] 
    public float stillFactor;
    public float crouchWalkingFactor;
    public float walkingFactor;
    public float runningFactor;
    
    // state
    private bool playerWithinSight;


    private void Update() {
        LookForPlayer();

        if (playerWithinSight) {
            IncreaseAwareness();
        }
        else if (Awareness > 0) Awareness -= Time.deltaTime / awarenessDecayTime;
        else Awareness = 0;

        UpdateEyesMaterial();
    }

    private void UpdateEyesMaterial() {
        Color color;
        float intensity;
        float minIntensity = 20f;
        float maxIntensity = 80f;
        if (Awareness >= 1) {
            color = Color.red;
            intensity = maxIntensity;
        }
        else if (Awareness >= 0.5f) {
            color = Color.red;
            intensity = Mathf.Lerp(minIntensity, maxIntensity, (Awareness - 0.5f) * 2f);
        }
        else if (Awareness > 0) {
            color = Color.Lerp(Color.yellow, Color.red, Awareness * 2f);
            intensity = minIntensity;
        }
        else {
            color = Color.yellow;
            intensity = minIntensity;
        }
        eyesMaterial.SetColor("_EmissionColor", color * Mathf.GammaToLinearSpace(intensity));
    }

    private void IncreaseAwareness() {
        float multiplier = 1f;
        Vector3 eyesPosition = transform.position + eyesOffset;
        Vector3 targetPosition = FirstPersonMovement.Instance.transform.position + targetOffset;
        
        // check distance
        float distance = Vector3.Distance(eyesPosition, targetPosition);
        if (distance < closeThreshold) multiplier *= closeFactor;
        else if (distance > farThreshold) multiplier *= farFactor;
        else multiplier *= mediumFactor;

        // check angle
        Vector3 directionToTarget = (targetPosition - eyesPosition).normalized;
        float angle = Vector3.Angle(transform.forward, directionToTarget);
        if (angle > sideThreshold) multiplier *= sideFactor;
        else multiplier *= frontFactor;
        
        // check player stance
        if (FirstPersonMovement.Instance.crouching) multiplier *= crouchedFactor;
        else multiplier *= standingFactor;

        // check player motion
        switch (FirstPersonMovement.Instance.moveState) {
            case FirstPersonMovement.MoveState.Still:
                multiplier *= stillFactor;
                break;
            case FirstPersonMovement.MoveState.CrouchWalking:
                multiplier *= crouchWalkingFactor;
                break;
            case FirstPersonMovement.MoveState.Walking:
                multiplier *= walkingFactor;
                break;
            case FirstPersonMovement.MoveState.Running:
                multiplier *= runningFactor;
                break;
        }
        
        // calculate awareness time
        float awarenessTime = baseAwarenessTime * multiplier;
        
        // calculate speed
        float speed = 1 / awarenessTime;

        // increment awareness
        Awareness += speed * Time.deltaTime;
        if (Awareness > 1) Awareness = 1;
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
            LastSeenPosition = targetPosition;
            return;
        }
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
