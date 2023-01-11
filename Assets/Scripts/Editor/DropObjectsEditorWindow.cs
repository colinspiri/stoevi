using UnityEngine;
using UnityEditor;
 
public class DropObjectsEditorWindow : EditorWindow
{
    // Add a menu item
    [ MenuItem("Window/Drop Object(s)") ]
 
    static void Awake()
    {
        // Get or create an editor window
        EditorWindow.GetWindow< DropObjectsEditorWindow>().Show();
    }
 
    void OnGUI()
    {
        GUILayout.Label("Drop using: ", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Bottom"))
        {
            DropObjects("Bottom");
        }
        if (GUILayout.Button("Origin"))
        {
            DropObjects("Origin");
        }
        if (GUILayout.Button("Center"))
        {
            DropObjects("Center");
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Raise and level object(s)")) {
            RaiseAndLevel();
        }
    }

    private void RaiseAndLevel() {
        Undo.RecordObjects( Selection.transforms, "Drop Objects" );

        for (int i = 0; i < Selection.transforms.Length; i++) {
            GameObject go = Selection.transforms[i].gameObject;
            if (!go)
                continue;
            
            // reset rotation and raise above terrain
            go.transform.rotation = Quaternion.Euler(0, go.transform.eulerAngles.y, 0);
            go.transform.Translate(2 * Vector3.up);
        }
    }
 
    private void DropObjects(string method) {
        Undo.RecordObjects( Selection.transforms, "Drop Objects" );

        foreach (var selectedTransform in Selection.transforms) {
            GameObject go = selectedTransform.gameObject;
            if ( !go )
                continue;

            Util.PlaceGameObjectOnTerrain(go, method);
        }
    }
}