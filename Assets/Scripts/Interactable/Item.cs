using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item", order = 0)]
public class Item : ScriptableObject {
    public string itemName;
    public float speedModifier;
    public GameObject pickupPrefab;
}