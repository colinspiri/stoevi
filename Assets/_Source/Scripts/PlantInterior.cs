using System.Collections.Generic;
using UnityEngine;

public class PlantInterior : MonoBehaviour {
    public GameObject interiorWallPrefab;

    public float interiorWallDistance = 1f;

    // private List<GameObject> interiors;
    
    // Kill all children to this gameobject
    public void DestroyAllInteriors()
    {
        List<GameObject> interiors = new List<GameObject>();

        // Find all interiors and store in temp list
        foreach (Transform child in transform)
        {
            if (child.CompareTag("PlantInterior")) {
                interiors.Add(child.gameObject);
            }
        }

        // Now destroy them
        foreach (GameObject child in interiors)
        {
            DestroyImmediate(child.gameObject);
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
