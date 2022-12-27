using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorbalanDirector : MonoBehaviour {
    // constants
    public float closeRadius;
    public float farRadius;
    public float maxTimeCloseToPlayer;
    public float maxTimeFarFromPlayer;
    public List<Transform> areaNodes;
    
    // state
    private bool directorCommandGiven;
    private float timeCloseToPlayer;
    private float timeFarFromPlayer;
    public Vector3 TargetPosition { get; set; }

    // Update is called once per frame
    void Update() {
        if (timeCloseToPlayer >= maxTimeCloseToPlayer) {
            directorCommandGiven = true;
            TargetPosition = GetFarthestAreaNodeFromPlayer();
            timeCloseToPlayer = 0;
        }
        if (timeFarFromPlayer >= maxTimeFarFromPlayer) {
            directorCommandGiven = true;
            TargetPosition = FirstPersonMovement.Instance.transform.position;
            timeFarFromPlayer = 0;
        }

        if (directorCommandGiven == false) {
            var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, transform.position);
            if (distance < closeRadius) {
                timeCloseToPlayer += Time.deltaTime;
                timeFarFromPlayer = 0;
                Debug.Log("timeCloseToPlayer = " + timeCloseToPlayer);
            }
            else if (distance > farRadius) {
                timeFarFromPlayer += Time.deltaTime;
                timeCloseToPlayer = 0;
                Debug.Log("timeFarFromPlayer = " + timeFarFromPlayer);
            }
        }
    }

    public Vector3 GetFarthestAreaNodeFromPlayer() {
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

    public bool GaveCommand() {
        return directorCommandGiven;
    }
    
    public void CompleteCommand() {
        directorCommandGiven = false;
        timeCloseToPlayer = 0;
        timeFarFromPlayer = 0;
        TargetPosition = transform.position;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, closeRadius);
        Gizmos.DrawWireSphere(transform.position, farRadius);
    }
}
