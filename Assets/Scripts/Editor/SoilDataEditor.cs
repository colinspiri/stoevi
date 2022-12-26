using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoilData))]
public class SoilDataEditor : Editor {
    private SoilData soilData;

    private void OnEnable() {
        soilData = target as SoilData;
    }

    public override void OnInspectorGUI() {
        // buttons
        if(GUILayout.Button("Clear Data")) {
            soilData.ClearData();
            soilData.SaveToFile();
        }

        if (GUILayout.Button("Add Random Crop")) {
            soilData.AddRandomCrop();
            soilData.SaveToFile();
        }
        
        // display original info
        base.OnInspectorGUI();
    }
}
