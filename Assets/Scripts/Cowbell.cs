using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cowbell : MonoBehaviour {
    private NavMeshAgent agent;
    private AudioSource cowbell;

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
        cowbell = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.velocity.magnitude < float.Epsilon) {
            cowbell.Stop();
        }
        else if (!cowbell.isPlaying) cowbell.Play();
    }
}
