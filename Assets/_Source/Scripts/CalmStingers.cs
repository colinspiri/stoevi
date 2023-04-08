using System.Collections;
using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;

public class CalmStingers : MonoBehaviour {
    public ASoundContainer mus_day1_calm;
    public float distanceThreshold;
    public float minWaitTime;
    public float maxWaitTime;
    
    private float distanceFromTorbalan;
    private float waitTimer;
    
    // Start is called before the first frame update
    void Start()
    {
        waitTimer = Random.Range(minWaitTime, maxWaitTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (TorbalanHearing.Instance != null && FirstPersonMovement.Instance != null) {
            distanceFromTorbalan = Vector3.Distance(FirstPersonMovement.Instance.transform.position, TorbalanHearing.Instance.transform.position);
            if (distanceFromTorbalan < distanceThreshold) return;
        }

        if (waitTimer <= 0) {
            mus_day1_calm.Play();
            waitTimer = Random.Range(minWaitTime, maxWaitTime);
        }
        else {
            waitTimer -= Time.deltaTime;
        }
    }
}
