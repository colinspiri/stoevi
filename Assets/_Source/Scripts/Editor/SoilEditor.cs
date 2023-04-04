using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Soil))]
public class SoilEditor : Editor {
    private Soil soil;

    private void OnEnable() {
        soil = target as Soil;
        EditorRefresh();
    }

    private void OnValidate() {
        EditorRefresh();
    }

    private void OnSceneGUI() {
        EditorRefresh();
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Refresh")) {
            EditorRefresh();
        }
    }

    private void EditorRefresh() {
        soil.LoadData();
    }
}
