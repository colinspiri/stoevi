using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

public class KillPlayer : Action
{ 
    // constants
    public Animator animator;
    public SharedFloat animationTime;
    public bool gameOverInEditor;
    
    // state
    private float timer;

    public override void OnStart() {
        base.OnStart();

        animator.SetBool("attack", true);
        timer = animationTime.Value;
    }

    public override TaskStatus OnUpdate() {
        // wait until animation is done
        if (timer > 0) {
            timer -= Time.deltaTime;
            return TaskStatus.Running;
        }

        return TaskStatus.Success;
    }

    public override void OnEnd() {
        base.OnEnd();
        
        timer = 0;
        if (!Application.isEditor || gameOverInEditor) {
            GameManager.Instance.GameOver(false);
        }
    }
}