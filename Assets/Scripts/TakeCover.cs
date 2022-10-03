using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeCover : MonoBehaviour
{
    // constants
    private float coverRadius = 5;
    private float coverAngle = 40f;
    
    // state
    private bool playerWasInCover;

    // Update is called once per frame
    void Update() {
        /*if (PlayerIsInCover()) {
            if (!playerWasInCover) {
                PlayerController.Instance.EnterCover();
                playerWasInCover = true;
            }
        }
        else if (playerWasInCover) {
            PlayerController.Instance.LeaveCover();
            playerWasInCover = false;
        }*/
    }

    /*private bool PlayerIsInCover() {
        // check distance
        var playerPos = PlayerController.Instance.transform.position;
        float distance = Vector3.Distance(transform.position, playerPos);
        if (distance > coverRadius) return false;
        
        // check if player is facing the cover
        Vector3 playerToCover = transform.position - playerPos;
        float facingAngle = Vector3.Angle(PlayerController.Instance.transform.forward, playerToCover);
        if (facingAngle > coverAngle) return false;
        
        // // check if torbalan is in front of player
        // Vector3 playerToTorbalan = TorbalanController.Instance.transform.position - playerPos;
        // float torbalanAngle = Vector3.Angle(playerToCover, playerToTorbalan);
        // if (torbalanAngle > coverAngle) return false;
        return false;
        // TODO

        return true;
    }*/
}
