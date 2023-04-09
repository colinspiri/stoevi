using UnityEngine;

[CreateAssetMenu(fileName = "TorbalanDirectorSettings", menuName = "TorbalanDirectorSettings")]
public class TorbalanDirectorSettings : ScriptableObject {
    [Header("Constants")]
    public float backstageDistance;
    public float frontstageDistance;
    public float frontstagePatrolRadius;
    public float intensityDistance;
    public int maxAggressionLevel;

    [Header("Calm")]
    [SerializeField] private float backstageTimeWhileCalm;
    [SerializeField] private float frontstageTimeWhileCalm;
    [SerializeField] private float maxIntensityWhileCalm;
    
    [Header("Aggressive")]
    [SerializeField] private float backstageTimeWhileAggressive; // 30
    [SerializeField] private float frontstageTimeWhileAggressive; // 60
    [SerializeField] private float maxIntensityWhileAggressive; // 60
    
    public float GetBackstageTime(int aggressionLevel) {
        return Mathf.Lerp(backstageTimeWhileCalm, backstageTimeWhileAggressive, (float)aggressionLevel / maxAggressionLevel);
    }
    public float GetFrontstageTime(int aggressionLevel) {
        return Mathf.Lerp(frontstageTimeWhileCalm, frontstageTimeWhileAggressive, (float)aggressionLevel / maxAggressionLevel);
    }
    public float GetMaxIntensity(int aggressionLevel) {
        return Mathf.Lerp(maxIntensityWhileCalm, maxIntensityWhileAggressive, (float)aggressionLevel / maxAggressionLevel);
    }
}