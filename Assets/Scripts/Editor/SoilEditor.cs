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
        UpdateSoil();
    }

    private void OnValidate() {
        Debug.Log("OnValidate()");
        UpdateSoil();
    }

    private void OnSceneGUI() {
        UpdateSoil();
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if(GUILayout.Button("Clear Data")) {
            soil.ClearData();
        }

        if (GUILayout.Button("Add Random Crop")) {
            AddRandomCrop();
        }
    }

    private void UpdateSoil() {
        Debug.Log("loading soil data on " + soil.name);
        
        soil.EditorChangedSoilData();
        soil.LoadData();
    }

    private void AddRandomCrop() {
        soil.soilData.AddRandomCrop();
        
        soil.EditorChangedSoilData();
        soil.LoadData();
    }
}
