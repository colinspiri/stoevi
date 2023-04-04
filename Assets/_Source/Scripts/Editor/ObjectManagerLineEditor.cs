using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectManagerLine))]
public class ObjectManagerLineEditor : Editor
{
    private ObjectManagerLine objectManager;

    private void OnEnable()
    {
        //This is a reference to the script
        objectManager = target as ObjectManagerLine;

        //Hide the handles of the GO
        Tools.hidden = true;
    }

    private void OnDisable()
    {
        //Unhide the handles of the GO
        Tools.hidden = false;
    }

    private void OnSceneGUI()
    {
        //Move the line's start and end positions and add objects if we have moved one of the positions

        //End position
        objectManager.endOfLinePos.y = Terrain.activeTerrain.SampleHeight(objectManager.endOfLinePos);
        objectManager.endOfLinePos = MovePoint(objectManager.endOfLinePos);

        //Start position
        objectManager.transform.position = new Vector3(objectManager.transform.position.x, Terrain.activeTerrain.SampleHeight(objectManager.transform.position), objectManager.transform.position.z);
        objectManager.transform.position = MovePoint(objectManager.transform.position);
    }

    private Vector3 MovePoint(Vector3 pos)
    {    
        //Change position
        if (Tools.current == Tool.Move)
        {
            //Check if we have moved the point
            EditorGUI.BeginChangeCheck();

            //Get the new position and display the position with axis
            pos = Handles.PositionHandle(pos, Quaternion.identity);

            //If we have moved the point
            if (EditorGUI.EndChangeCheck())
            {
                MarkSceneAsDirty();

                UpdateObjects();
            }
        }

        return pos;
    }

    //Update the objects between the path
    private void UpdateObjects()
    {
        //First kill all current objects
        objectManager.KillAllChildren();

        //How many object fit between the points?

        //Make sure the size of the object is not zero because then we can fit infinite amount of objects
        if (objectManager.objectSize == 0f)
        {
            return;
        }

        //The distance between the points
        float distanceBetween = (objectManager.endOfLinePos - objectManager.transform.position).magnitude;

        //If we divide the distance between the points and the size of one object we know how many can fit between 
        int objects = Mathf.FloorToInt(distanceBetween / objectManager.objectSize);

        //The direction between the points
        Vector3 direction = (objectManager.endOfLinePos - objectManager.transform.position).normalized;

        //Where should we instantiate the first object
        Vector3 instantiatePos = objectManager.transform.position + direction * (objectManager.objectSize * 0.5f);
        instantiatePos.y = objectManager.transform.position.y + 5;

        //Add the objects
        for (int i = 0; i < objects; i++)
        {
            GameObject newGO = PrefabUtility.InstantiatePrefab(objectManager.prefabGO) as GameObject;

            //Parent it so we can delete it by killing all children
            newGO.transform.parent = objectManager.transform;

            // Give it the position
            newGO.transform.position = instantiatePos;
            
            // Place it on terrain
            Util.PlaceGameObjectOnTerrain(newGO, "Bottom");

            //Orient it by making it look at the position we are going to
            newGO.transform.forward = direction;

            //Move to the next object
            instantiatePos += direction * objectManager.objectSize;
        }
    }

    //Force unity to save changes or Unity may not save when we have instantiated/removed prefabs despite pressing save button
    private void MarkSceneAsDirty()
    {
        UnityEngine.SceneManagement.Scene activeScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(activeScene);
    }
}