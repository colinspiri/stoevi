using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;

[CreateAssetMenu(fileName = "TorbalanDirectorSettings", menuName = "TorbalanDirectorSettings")]
public class TorbalanDirectorSettings : ScriptableObject {
    [Header("Backstage")]
    public float backstageDistance;
    public float maxBackstageTime;

    [Header("Frontstage")] 
    public float frontstageDistance;
    public float maxFrontstageTime;

    [Header("Intensity")]
    public float intensityRadius;
    public float maxIntensity;
}