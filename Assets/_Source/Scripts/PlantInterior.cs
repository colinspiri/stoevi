using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantInterior : MonoBehaviour {
    public GameObject interiorWallPrefab;

    public float interiorWallDistance = 1f;

    public List<GameObject> interiors;
    
    // Kill all children to this gameobject
    public void DestroyAllInteriors()
    {
        /*//Get an array with all children to this transform
        GameObject[] allChildren = GetAllChildren();

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child);
        }*/

        foreach (var interior in interiors) {
            DestroyImmediate(interior);
        }
    }

    //Get an array with all children to this GO
    private GameObject[] GetAllChildren()
    {
        //This array will hold all children
        GameObject[] allChildren = new GameObject[transform.childCount];

        //Fill the array
        int childCount = 0;
        foreach (Transform child in transform)
        {
            allChildren[childCount] = child.gameObject;
            childCount += 1;
        }

        return allChildren;
    }
}
