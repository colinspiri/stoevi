using UnityEngine;

public class Pickup : Interactable {
    public Item item;
    public HeldItem heldItem;

    public override void Interact() {
        heldItem.PickUpItem(item);
        Destroy(gameObject);
    }

    public override string GetObjectName() {
        return item.itemName;
    }

    public override string GetObjectDescription() {
        return "";
    }

    public override string GetButtonPrompt() {
        return GetInteractButton() + " to pick up " + item.itemName;
    }
}