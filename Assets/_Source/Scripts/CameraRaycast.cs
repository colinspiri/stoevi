using System;
using UnityEngine;

public class CameraRaycast : MonoBehaviour {
    public static CameraRaycast Instance;

    // The maximum distance that the raycast should check for an object
    public LayerMask terrainLayer;
    public float maxRaycastDistance;

    private void Awake() {
        Instance = this;
    }

    public Vector3 GetCurrentInteractableHitPosition() {
        RaycastHit hitInfo;
        var interactable = GetCurrentInteractable(out hitInfo);
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
                // ignore hits against player
                if (hit.collider.gameObject.CompareTag("Player")) {
                    continue;
                }
                
                hitInfo = hit;
                return null;
            }
        }

        hitInfo = new RaycastHit();
        return null;
    }

    public bool GetTerrainHitPosition(float maxDistanceToPlayer, out Vector3 hitPosition) {
        // Create a ray that starts at the camera's position and points in the direction that the camera is facing
        Ray ray = new Ray(transform.position, transform.forward);

        // raycast all and sort by distance to camera
        RaycastHit hitInfo;
        bool hitTerrain = Physics.Raycast(ray, out hitInfo, maxRaycastDistance, terrainLayer);
        hitPosition = hitInfo.point;
        if (!hitTerrain) return false;

        float distance = Vector3.Distance(hitPosition, FirstPersonMovement.Instance.transform.position);
        return distance < maxDistanceToPlayer;
    }
}
