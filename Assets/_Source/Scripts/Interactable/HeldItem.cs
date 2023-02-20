using UnityEngine;
using UnityEngine.ProBuilder;

[CreateAssetMenu(fileName = "NewHeldItem", menuName = "HeldItem", order = 0)]
public class HeldItem : ScriptableObject {
    public float dropItemDistance;
    public Item heldItem { get; private set; }

    public bool HoldingItem() {
        return heldItem != null;
    }
    public void PickUpItem(Item item) {
        heldItem = item;
    }
    public void DropItem(Transform dropTransform = null) {
        // transform given
        if (dropTransform != null) {
            Instantiate(heldItem.pickupPrefab, dropTransform.position, dropTransform.rotation);
        }
        // looking at terrain
        else if(CameraRaycast.Instance && CameraRaycast.Instance.GetTerrainHitPosition(dropItemDistance, out Vector3 hitPosition)) {
            Instantiate(heldItem.pickupPrefab, hitPosition, Quaternion.identity);
        }
        // drop in front of player
        else {
            Vector3 dropPosition = FirstPersonMovement.Instance.transform.position +
                                   dropItemDistance * FirstPersonMovement.Instance.transform.forward;
            Instantiate(heldItem.pickupPrefab, dropPosition, Quaternion.identity);
        }

        // change state
        heldItem = null;
    }

    public void RemoveItem() {
        heldItem = null;
    }
}