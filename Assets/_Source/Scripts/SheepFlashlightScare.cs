using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class SheepFlashlightScare : MonoBehaviour {
    public Sheep sheep;

    public float maxDistance;
    public float maxAngle;

    // Update is called once per frame
    void Update()
    {
        if (Flashlight.Instance != null && Flashlight.Instance.flashlightOn) {
            // check distance
            float distance = Vector3.Distance(transform.position, Flashlight.Instance.transform.position);
            if (distance > maxDistance) return;
            
            // check flashlight facing sheep
            Vector3 directionToTarget = (transform.position - Flashlight.Instance.transform.position).normalized;
            float angle = Vector3.Angle(Flashlight.Instance.transform.forward, directionToTarget);

            if (angle > maxAngle / 2) return;

            sheep.BecomeScared();
        }
    }
}
