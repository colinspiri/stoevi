using System.Collections;
using System.Collections.Generic;
using PlasticGui;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlantInterior))]
public class PlantInteriorEditor : Editor
{
    private PlantInterior plantInterior;

    private void OnEnable()
    {
        //This is a reference to the script
        plantInterior = target as PlantInterior;
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        if(GUILayout.Button("Create Interior")) {
            MarkSceneAsDirty();
            UpdateInterior();
        }
        if(GUILayout.Button("Delete Interior")) {
            plantInterior.KillAllChildren();
        }
    }

    private void UpdateInterior() {
        // remove all previous interior objects
        plantInterior.KillAllChildren();

        Bounds bounds = plantInterior.GetComponent<Renderer>().bounds;
        float borderWallOffset = 0.35f;
        
        // along X
        float startingX = plantInterior.transform.position.x - bounds.extents.x + borderWallOffset;
        CreateWall(startingX, 0, true);
        
        float endingX = plantInterior.transform.position.x + bounds.extents.x - borderWallOffset;
        CreateWall(endingX, 0, true);
        
        int wallsAlongX = Mathf.FloorToInt((endingX - startingX) / plantInterior.interiorWallDistance);
        float evenDistanceX = (endingX - startingX) / wallsAlongX;
        for (float wallX = startingX; wallX < endingX; wallX += evenDistanceX) {
            CreateWall(wallX, 0, true);
        }
        
        // along X
        float startingZ = plantInterior.transform.position.z - bounds.extents.z + borderWallOffset;
        CreateWall(0, startingZ);
        
        float endingZ = plantInterior.transform.position.z + bounds.extents.z - borderWallOffset;
        CreateWall(0, endingZ);
        
        int wallsAlongZ = Mathf.FloorToInt((endingZ - startingZ) / plantInterior.interiorWallDistance);
        float evenDistanceZ = (endingZ - startingZ) / wallsAlongZ;
        for (float wallZ = startingZ; wallZ < endingZ; wallZ += evenDistanceZ) {
            CreateWall( 0, wallZ);
        }
    }

    private void CreateWall(float x, float z, bool perpendicular = false) {
        Vector3 position = new Vector3(x, plantInterior.transform.position.y, z);
        if (position.x == 0) position.x = plantInterior.transform.position.x;
        if (position.z == 0) position.z = plantInterior.transform.position.z;
        GameObject wall = PrefabUtility.InstantiatePrefab(plantInterior.interiorWallPrefab, plantInterior.transform) as GameObject;
        wall.transform.position = position;
        wall.transform.rotation = plantInterior.transform.rotation;

        // Util.PlaceGameObjectOnTerrain(wall, "Bottom");
        
        if(perpendicular) wall.transform.Rotate(Vector3.up, 90);
    } 

    //Force unity to save changes or Unity may not save when we have instantiated/removed prefabs despite pressing save button
    private void MarkSceneAsDirty()
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
    }
}