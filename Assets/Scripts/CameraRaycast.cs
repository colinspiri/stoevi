using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaycast : MonoBehaviour {
    // The maximum distance that the raycast should check for an object
    public float maxRaycastDistance;

    // The layer mask to use for the raycast. This should include the layers of any objects that you want to detect.
    public LayerMask detectionLayerMask;

    // Update is called once per frame
    void Update() {
        // Create a ray that starts at the camera's position and points in the direction that the camera is facing
        Ray ray = new Ray(transform.position, transform.forward);

        // Perform a raycast using the ray and the specified maximum distance and layer mask
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, maxRaycastDistance, detectionLayerMask);

        // If the raycast hits an object, print a message to the console
        if (hit && hitInfo.collider.gameObject.GetComponent<Interactable>() != null) {
            var interactable = hitInfo.collider.gameObject.GetComponent<Interactable>();

            if (interactable.PlayerWithinInteractionDistance(hitInfo.point)) {
                InteractableManager.Instance.SelectObject(interactable);
                return;
            }
        }
        
        InteractableManager.Instance.SelectObject(null);
    }
}
