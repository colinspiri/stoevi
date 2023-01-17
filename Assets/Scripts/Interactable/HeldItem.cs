using UnityEngine;

[CreateAssetMenu(fileName = "NewHeldItem", menuName = "HeldItem", order = 0)]
public class HeldItem : ScriptableObject {
    public Item heldItem { get; private set; }

    public bool HoldingItem() {
        return heldItem != null;
    }
    public void PickUpItem(Item item) {
        heldItem = item;
    }
    public void DropItem() {
        // spawn prefab
        Vector3 dropPosition = FirstPersonMovement.Instance.transform.position +
                               2 * FirstPersonMovement.Instance.transform.forward;
        Instantiate(heldItem.pickupPrefab, dropPosition, Quaternion.identity);
        
        // change state
        heldItem = null;
    }
}