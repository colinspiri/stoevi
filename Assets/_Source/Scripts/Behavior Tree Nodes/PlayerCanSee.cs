using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerCanSee : Conditional {
    public SharedGameObject obj;
    public SharedFloat maxAngle;

    public override TaskStatus OnUpdate() {
        Vector3 eyesPosition = FirstPersonMovement.Instance.GetRaycastTarget();
        Vector3 directionToTarget = (transform.position - eyesPosition).normalized;
        float angle = Vector3.Angle(FirstPersonMovement.Instance.transform.forward, directionToTarget);
        bool withinView = angle < maxAngle.Value / 2;
        
        Debug.Log("angle = " + angle);

        return withinView ? TaskStatus.Success : TaskStatus.Failure;
    }
}