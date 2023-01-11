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
    public static TorbalanVision Instance;
    public Material eyesMaterial;
    public Light eyeLight;
    
    // synced with behavior tree
    public bool PlayerWithinVision { get; set; }
    public float Awareness => torbalanAwareness.Value;

    // constants
    public bool blind;
    public Vector3 eyesOffset;
    public LayerMask obstacleMask;
    [Header("Normal Vision")]
    public float normalVisionDistance;
    [Range(0, 360)] public float normalVisionAngle;
    [Header("Peripheral Vision")]
    public float peripheralVisionDistance;
    [Range(0, 360)] public float peripheralVisionAngle;
    [Header("Close Vision")]
    public float closeVisionDistance;
    [Range(0, 360)] public float closeVisionAngle;

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
    public FloatVariable torbalanAwareness;
    private bool playerInNormalVision;
    private bool playerInPeripheralVision;
    private bool playerInCloseVision;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        torbalanAwareness.SetValue(0);
    }

    private void Update() {
        LookForPlayer();

        if (PlayerWithinVision) {
            IncreaseAwareness();
        }
        else if (torbalanAwareness.Value > 0) torbalanAwareness.ApplyChange(-Time.deltaTime / awarenessDecayTime);
        else torbalanAwareness.SetValue(0);

        UpdateEyeLights();
    }

    private void UpdateEyeLights() {
        Color color;
        float intensity;
        float minIntensity = 20f;
        float maxIntensity = 80f;
        float brightness;
        float minBrightness = 50f;
        float maxBrightness = 100f;
        if (torbalanAwareness.Value >= 1) {
            color = Color.red;
            intensity = maxIntensity;
            brightness = maxBrightness;
        }
        else if (torbalanAwareness.Value >= 0.5f) {
            color = Color.red;
            float t = (torbalanAwareness.Value - 0.5f) * 2f;
            intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
            brightness = Mathf.Lerp(minBrightness, maxBrightness, t);
        }
        else if (torbalanAwareness.Value > 0) {
            float t = torbalanAwareness.Value * 2f;
            color = Color.Lerp(Color.yellow, Color.red, t);
            intensity = minIntensity;
            brightness = minBrightness;
        }
        else {
            color = Color.yellow;
            intensity = minIntensity;
            brightness = minBrightness;
        }
        eyesMaterial.SetColor("_EmissionColor", color * Mathf.GammaToLinearSpace(intensity));
        eyeLight.color = color;
        eyeLight.intensity = brightness;
    }

    private void IncreaseAwareness() {
        float multiplier = 1f;
        Vector3 eyesPosition = transform.position + eyesOffset;
        Vector3 targetPosition = FirstPersonMovement.Instance.GetRaycastTarget();
        
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
        torbalanAwareness.ApplyChange(speed * Time.deltaTime);
        if (torbalanAwareness.Value > 1) torbalanAwareness.SetValue(1);
    }

    private void LookForPlayer() {
        if (blind) {
            playerInNormalVision = false;
            playerInPeripheralVision = false;
            playerInCloseVision = false;
            return;
        }

        playerInNormalVision = CheckIfPlayerWithinCone(normalVisionDistance, normalVisionAngle);
        playerInPeripheralVision = CheckIfPlayerWithinCone(peripheralVisionDistance, peripheralVisionAngle);
        playerInCloseVision = CheckIfPlayerWithinCone(closeVisionDistance, closeVisionAngle);

        PlayerWithinVision = playerInNormalVision || playerInPeripheralVision || playerInCloseVision;
    }

    public bool CanSeePointInFocusedVision(Vector3 point) {
        bool pointInNormalVision = CheckIfPointWithinCone(point, normalVisionDistance / 2, normalVisionAngle);
        // bool pointInPeripheralVision = CheckIfPointWithinCone(point, peripheralVisionDistance, peripheralVisionAngle);
        bool pointInCloseVision = CheckIfPointWithinCone(point, closeVisionDistance, closeVisionAngle);

        return pointInNormalVision || pointInCloseVision;
    }

    public bool CanSeePointInAnyVision(Vector3 point) {
        bool pointInNormalVision = CheckIfPointWithinCone(point, normalVisionDistance, normalVisionAngle);
        bool pointInPeripheralVision = CheckIfPointWithinCone(point, peripheralVisionDistance, peripheralVisionAngle);
        bool pointInCloseVision = CheckIfPointWithinCone(point, closeVisionDistance, closeVisionAngle);

        return pointInNormalVision || pointInPeripheralVision || pointInCloseVision;
    }

    private bool CheckIfPlayerWithinCone(float distance, float angle) {
        Vector3 targetPosition = FirstPersonMovement.Instance.GetRaycastTarget();
        return CheckIfPointWithinCone(targetPosition, distance, angle);
    }
    
    private bool CheckIfPointWithinCone(Vector3 point, float distance, float angle) {
        Vector3 eyesPosition = transform.position + eyesOffset;
        
        // check if within distance
        Vector3 directionToTarget = (point - eyesPosition).normalized;
        float distanceToTarget = Vector3.Distance(eyesPosition, point);
        if (distanceToTarget > distance) return false;

        // check if within view angle
        if (Vector3.Angle(transform.forward, directionToTarget) > angle / 2) return false;
        
        // check if obstacles between self and point
        if (Physics.Raycast(eyesPosition, directionToTarget, distanceToTarget, obstacleMask, QueryTriggerInteraction.Collide)) return false;

        return true;
    }

    private Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal) {
        if (!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void SetBlind(bool value) {
        blind = value;
    }

    private void OnDrawGizmos() {
        DrawVisionCone(normalVisionDistance, normalVisionAngle, Color.white);
        DrawVisionCone(peripheralVisionDistance, peripheralVisionAngle, Color.blue);
        DrawVisionCone(closeVisionDistance, closeVisionAngle, Color.magenta);

        // if seen, line to player
        Gizmos.color = Color.red;
        if (playerInNormalVision) {
            Vector3 eyesPosition = transform.position + eyesOffset;
            Gizmos.DrawLine(eyesPosition, FirstPersonMovement.Instance.GetRaycastTarget());
        }
    }

    private void DrawVisionCone(float distance, float angle, Color color) {
        Vector3 eyesPosition = transform.position + eyesOffset;
        
        Vector3 viewAngleA = DirectionFromAngle(-angle / 2, false);
        Vector3 viewAngleB = DirectionFromAngle(angle / 2, false);
        
        Gizmos.color = color;
        Gizmos.DrawLine(eyesPosition, eyesPosition + viewAngleA * distance);
        Gizmos.DrawLine(eyesPosition, eyesPosition + viewAngleB * distance);
    }
}
