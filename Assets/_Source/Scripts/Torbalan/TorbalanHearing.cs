using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class TorbalanHearing : MonoBehaviour {
    // components
    public static TorbalanHearing Instance;
    public NavMeshAgent agent;
    public BehaviorTree behavior;
    public AudioSource torbalan_inhale;

    // synced with behavior tree
    public Vector3 LastHeardLocation { get; set; }
    public bool HeardRecently { get; set; }

    // constants
    public bool deaf;
    public float heardTime;
    
    // state
    private float heardTimer;
    
    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (heardTimer > 0) {
            heardTimer -= Time.deltaTime;
        }
        HeardRecently = heardTimer > 0;
    }

    public void ReportSound(Vector3 soundOrigin, float loudness) {
        if (deaf) return;
        
        var path = new NavMeshPath();
        bool pathFound = agent.CalculatePath(soundOrigin, path);
        if (!pathFound || path.status == NavMeshPathStatus.PathPartial || path.status == NavMeshPathStatus.PathInvalid) return;
        var length = GetPathLength(path);
        
        if (length <= loudness) {
            heardTimer = heardTime;
            LastHeardLocation = soundOrigin;

            if (!torbalan_inhale.isPlaying) {
                torbalan_inhale.Play();
            }
            
            behavior.SetVariableValue("Last Known Position", FirstPersonMovement.Instance.transform.position);
            behavior.SendEvent("LastKnownPositionUpdated");
        }
    }

    private float GetPathLength(NavMeshPath path) {
        if (path.status != NavMeshPathStatus.PathComplete) return 0;
        
        float length = 0;
        for (int i = 1; i < path.corners.Length; i++) {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }

        return length;
    }

    public void SetDeaf(bool value) {
        deaf = value;
    }
}
