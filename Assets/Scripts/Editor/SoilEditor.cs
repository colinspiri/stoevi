using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Soil))]
public class SoilEditor : Editor {
    private Soil soil;

    private void OnEnable() {
        soil = target as Soil;
        LoadSoilData();
    }

    private void OnValidate() {
        LoadSoilData();
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        if(GUILayout.Button("Clear Data")) {
            soil.ClearData();
        }
    }

    private void LoadSoilData() {
        Debug.Log("loading soil data on " + soil.name);
        soil.LoadData();
    }
}
