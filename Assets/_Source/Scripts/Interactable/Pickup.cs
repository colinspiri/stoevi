
public class Pickup : Interactable {
    public Item item;
    public HeldItem heldItem;

    public override void InteractPrimary() {
        // if holding something, first drop it
        if (heldItem.HoldingItem()) {
            heldItem.DropItem(transform);
        }
        // pick up item
        heldItem.PickUpItem(item);
        // destroy self
        Destroy(gameObject);
    }

    public override string GetObjectName() {
        return item.itemName;
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPromptPrimary() {
        return GetInteractPrimaryButton() + " to pick up " + item.itemName;
    }
}