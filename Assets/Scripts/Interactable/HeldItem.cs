using UnityEngine;
using UnityEngine.ProBuilder;

[CreateAssetMenu(fileName = "NewHeldItem", menuName = "HeldItem", order = 0)]
public class HeldItem : ScriptableObject {
    public Item heldItem { get; private set; }

    public bool HoldingItem() {
        return heldItem != null;
    }
    public void PickUpItem(Item item) {
        heldItem = item;
    }
    public void DropItem(Transform dropTransform = null) {
        // spawn prefab
        if (dropTransform != null) {
            Instantiate(heldItem.pickupPrefab, dropTransform.position, dropTransform.rotation);
        }
        else {
            Vector3 dropPosition = FirstPersonMovement.Instance.transform.position +
                                2 * FirstPersonMovement.Instance.transform.forward;
            Instantiate(heldItem.pickupPrefab, dropPosition, Quaternion.identity);
        }

        // change state
        heldItem = null;
    }
}