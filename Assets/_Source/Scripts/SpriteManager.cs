using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpriteManager : MonoBehaviour {
    // components
    public MeshRenderer meshRenderer;
    
    // state
    private Material front;
    private Material back;
    private Material left;
    private Material right;

    // Update is called once per frame
    protected void Update() {
        Vector3 toTarget = CameraRaycast.Instance.transform.position - transform.position;
        toTarget.Normalize();
        float dot = Vector3.Dot(toTarget, transform.forward);

        if (dot < -0.75) {
            meshRenderer.material = back;
        }
        else if (dot < 0.75) {
            float angle = Vector3.SignedAngle(toTarget, transform.forward, Vector3.up);
            meshRenderer.material = angle < 0 ? left : right;
        }
        else {
            meshRenderer.material = front;
        }
    }

    protected void ChangeMaterials(Material newFront, Material newBack, Material newLeft, Material newRight) {
        front = newFront;
        back = newBack;
        left = newLeft;
        right = newRight;
    }
}
