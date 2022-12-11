using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityPhysics;
using UnityEngine;

public class CameraRaycast : MonoBehaviour {
    public static CameraRaycast Instance;

    // The maximum distance that the raycast should check for an object
    public float maxRaycastDistance;

    // The layer mask to use for the raycast. This should include the layers of any objects that you want to detect.
    public LayerMask detectionLayerMask;

    private void Awake() {
        Instance = this;
    }

    public Vector3 GetCurrentHitPosition() {
        RaycastHit hitInfo;
        var interactable = GetCurrentObject(out hitInfo);
        return hitInfo.point;
    }

    public GameObject GetCurrentObject(out RaycastHit hitInfo) {
        // Create a ray that starts at the camera's position and points in the direction that the camera is facing
        Ray ray = new Ray(transform.position, transform.forward);

        // Perform a raycast using the ray and the specified maximum distance and layer mask
        bool hit = Physics.Raycast(ray, out hitInfo, maxRaycastDistance, detectionLayerMask);

        // If the raycast hits an object
        if (hit) {
            return hitInfo.collider.gameObject;
        }

        return null;
    }

    public bool LookingAtGround(out RaycastHit hitInfo) {
        // Create a ray that starts at the camera's position and points in the direction that the camera is facing
        Ray ray = new Ray(transform.position, transform.forward);

        // Perform a raycast using the ray and the specified maximum distance
        bool hit = Physics.Raycast(ray, out hitInfo, maxRaycastDistance);

        // If the raycast hits an object
        if (hit && hitInfo.collider.CompareTag("Terrain")) {
            return true;
        }

        return false;
    }
}
