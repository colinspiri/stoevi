using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityPhysics;
using UnityEngine;

public class CameraRaycast : MonoBehaviour {
    public static CameraRaycast Instance;

    // The maximum distance that the raycast should check for an object
    public float maxRaycastDistance;

    private void Awake() {
        Instance = this;
    }

    public Vector3 GetCurrentHitPosition() {
        RaycastHit hitInfo;
        var interactable = GetCurrentObject(out hitInfo);
        return hitInfo.point;
    }

    public Interactable GetCurrentInteractable(out RaycastHit hitInfo) {
        // Create a ray that starts at the camera's position and points in the direction that the camera is facing
        Ray ray = new Ray(transform.position, transform.forward);

        // raycast all and sort by distance to camera
        RaycastHit[] allHits = Physics.RaycastAll(ray, maxRaycastDistance, Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Collide);
        Array.Sort(allHits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));
        
        // iterate through raycast hits
        foreach (var hit in allHits) {
            // hit interactable
            var hitInteractable = hit.transform.GetComponent<Interactable>();
            if (hitInteractable != null) {
                hitInfo = hit;
                return hitInteractable;
            }
            
            // hit collider 
            if (hit.collider.isTrigger == false) {
                hitInfo = hit;
                return null;
            }
        }

        hitInfo = new RaycastHit();
        return null;
    }

    public GameObject GetCurrentObject(out RaycastHit hitInfo) {
        // Create a ray that starts at the camera's position and points in the direction that the camera is facing
        Ray ray = new Ray(transform.position, transform.forward);

        // Perform a raycast using the ray and the specified maximum distance and layer mask
        bool hit = Physics.Raycast(ray, out hitInfo, maxRaycastDistance);

        // If the raycast hits an object
        if (hit) {
            return hitInfo.collider.gameObject;
        }

        return null;
    }
}
