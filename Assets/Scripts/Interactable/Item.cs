using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item", order = 0)]
public class Item : ScriptableObject {
    public string itemName;
    public GameObject pickupPrefab;
    public float speedModifier = 1f;
}