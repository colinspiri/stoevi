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
        }

        if (GUILayout.Button("Add Random Crop")) {
            soilData.AddRandomCrop();
        }
        
        // display original info
        base.OnInspectorGUI();
    }
}
