using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Lighting Preset")]
public class LightingPreset : ScriptableObject {
    public Gradient AmbientColor;
    public Gradient DirectionalColor;
    public Gradient FogColor;
    public float FogMaxDistanceDay;
    public float FogMaxDistanceNight;
    [Range(0, 360)] public float StartingLightDirection;
    [Range(0, 360)] public float EndingLightDirection;
}
