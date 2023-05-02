using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class KillPlayer : Action
{ 
    // constants
    public bool gameOverInEditor;

    public override TaskStatus OnUpdate() {
        if (!Application.isEditor || gameOverInEditor) {
            GameManager.Instance.GameOver(false);
        }
        return TaskStatus.Success;
    }
}