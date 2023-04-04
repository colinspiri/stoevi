using UnityEngine;
using UnityEngine.AI;

public class Cowbell : MonoBehaviour {
    public NavMeshAgent agent;
    private AudioSource cowbell;

    private void Awake() {
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
