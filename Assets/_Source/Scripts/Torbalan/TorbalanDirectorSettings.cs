using UnityEngine;

[CreateAssetMenu(fileName = "TorbalanDirectorSettings", menuName = "TorbalanDirectorSettings")]
public class TorbalanDirectorSettings : ScriptableObject {
    [Header("Backstage")]
    public float backstageDistance;
    [SerializeField] private float backstageTimeWhileCalm;
    [SerializeField] private float backstageTimeWhileAggressive; // 30

    [Header("Frontstage")] 
    public float frontstageDistance;
    public float frontstagePatrolRadius;
    [SerializeField] private float frontstageTimeWhileCalm;
    [SerializeField] private float frontstageTimeWhileAggressive; // 60

    [Header("Intensity")]
    public float intensityDistance;
    [SerializeField] private float maxIntensityWhileCalm;
    [SerializeField] private float maxIntensityWhileAggressive; // 60

    [Header("Aggression")] 
    public int maxAggressionLevel;

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