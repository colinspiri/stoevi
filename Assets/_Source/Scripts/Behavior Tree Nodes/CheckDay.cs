using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class CheckDay : Conditional {
    public int day;
    
    public override TaskStatus OnUpdate() {
        int currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        
        return currentDay == day ? TaskStatus.Success : TaskStatus.Failure;
    }
}