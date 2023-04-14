using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class CheckDay : Conditional {
    public int day;
    public enum Comparison { Equal, GreaterThanOrEqual }
    public Comparison comparison;
    
    public override TaskStatus OnUpdate() {
        int currentDay = PlayerPrefs.GetInt("CurrentDay", 1);

        if (comparison == Comparison.Equal) {
            return currentDay == day ? TaskStatus.Success : TaskStatus.Failure;
        }
        else if (comparison == Comparison.GreaterThanOrEqual) {
            return currentDay >= day ? TaskStatus.Success : TaskStatus.Failure;
        }

        return TaskStatus.Failure;
    }
}