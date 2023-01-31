using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteFaceCamera : MonoBehaviour {
    public float offsetInDegrees = 0f;
    
    // Update is called once per frame
    void Update() {
        Vector3 vectorToTarget = Camera.main.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.x, vectorToTarget.z) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle + offsetInDegrees, Vector3.up);
        transform.rotation = q;
    }
}
