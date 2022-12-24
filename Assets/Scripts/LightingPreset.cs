using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Lighting Preset", menuName = "Lighting Preset")]
public class LightingPreset : ScriptableObject {
    public Gradient AmbientColor;
    public Gradient DirectionalColor;
    public Gradient FogColor;
}
