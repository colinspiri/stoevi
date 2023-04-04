using UnityEngine;

public class ObjectManagerLine : MonoBehaviour 
{
    //The object we want to add
    public GameObject prefabGO;

    //Whats the size of the prefab we want to add?
    //You can increase the size if you want to have a gap between the objects
    public float objectSize;

    //Where is the line ending? It starts at the position of the gameobject the script is attached to
    public Vector3 endOfLinePos;

    //Kill all children to this gameobject
    public void KillAllChildren()
    {
        //Get an array with all children to this transform
        GameObject[] allChildren = GetAllChildren();

        //Now destroy them
        foreach (GameObject child in allChildren)
        {
            DestroyImmediate(child);
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