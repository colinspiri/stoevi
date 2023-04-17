using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {
    private Vector3 offset;
    
    // Start is called before the first frame update
    void Start() {
        if (FirstPersonMovement.Instance != null) {
            offset = transform.position - FirstPersonMovement.Instance.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (FirstPersonMovement.Instance != null) {
            transform.position = FirstPersonMovement.Instance.transform.position + offset;
        }
    }
}
