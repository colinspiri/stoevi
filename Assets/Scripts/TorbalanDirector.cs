using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TorbalanDirector : MonoBehaviour {
    // constants
    public float closeRadius;
    public float farRadius;
    public float maxTimeCloseToPlayer;
    public float maxTimeFarFromPlayer;
    public List<Transform> areaNodes;
    
    // behavior tree variables
    public Vector3 TargetPosition { get; set; }
    public bool CommandGiven;
    
    // state
    public enum DirectorCommand { None, ApproachPlayer, BackOff }
    private DirectorCommand currentCommand = DirectorCommand.None;
    private float timeCloseToPlayer;
    private float timeFarFromPlayer;

    // Update is called once per frame
    void Update() {
        if (currentCommand == DirectorCommand.None) {
            CountTimers();
        }
        else if (currentCommand == DirectorCommand.ApproachPlayer) {
            TargetPosition = FirstPersonMovement.Instance.transform.position;
            CheckApproachPlayerCondition();
        }
        else if (currentCommand == DirectorCommand.BackOff) {
            TargetPosition = GetFarthestAreaNodeFromPlayer();
            CheckBackOffCondition();
        }
    }

    private void CountTimers() {
        // calculate distance from player
        var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, transform.position);
            
        // close timer
        if (distance < closeRadius) {
            timeCloseToPlayer += Time.deltaTime;
                
            if (timeCloseToPlayer >= maxTimeCloseToPlayer) {
                timeCloseToPlayer = 0;
                timeFarFromPlayer = 0;
                currentCommand = DirectorCommand.BackOff;
                CommandGiven = true;
            }
        }

        // far timer
        if (distance > farRadius) {
            timeFarFromPlayer += Time.deltaTime;
                
            if (timeFarFromPlayer >= maxTimeFarFromPlayer) {
                timeCloseToPlayer = 0;
                timeFarFromPlayer = 0;
                currentCommand = DirectorCommand.ApproachPlayer;
                CommandGiven = true;
            }
        }
    }

    private void CheckApproachPlayerCondition() {
        var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, transform.position);
        if (distance <= closeRadius) {
            CompleteCommand();
        }
    }

    private void CheckBackOffCondition() {
        var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, transform.position);
        if (distance >= farRadius) {
            CompleteCommand();
        }
    }

    public void CompleteCommand() {
        currentCommand = DirectorCommand.None;
        CommandGiven = false;
        TargetPosition = transform.position;
    }
    
    private Vector3 GetFarthestAreaNodeFromPlayer() {
        float maxDistance = 0;
        Vector3 farthestPosition = Vector3.zero;
        foreach (var node in areaNodes) {
            var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, node.position);
            if (distance > maxDistance) {
                distance = maxDistance;
                farthestPosition = node.position;
            }
        }
        return farthestPosition;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, closeRadius);
        Gizmos.DrawWireSphere(transform.position, farRadius);
    }
}
