using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Interactable : MonoBehaviour {
    // constants
    [FormerlySerializedAs("interactionDistance")] public float InteractionDistance = 5f;
    [FormerlySerializedAs("interactionTime")] public float InteractionTime = 1f;
    
    // state
    private bool selectable = true;
    private bool interactable = true;

    // Start is called before the first frame update
    protected virtual void Start() {
        InteractableManager.Instance.AddInteractable(this);
    }

    public abstract void Interact();
    public abstract string GetUIText();
    public virtual float GetSliderFloat() { return 0; }

    public bool PlayerWithinInteractionDistance(Vector3 raycastPoint) {
        var distance = Vector3.Distance(FirstPersonController.Instance.transform.position, raycastPoint);
        return distance <= InteractionDistance;
    }

    protected void SetSelectable(bool value) {
        selectable = value;
    }
    public bool IsSelectable() {
        return selectable;
    }
    protected void SetInteractable(bool value) {
        interactable = value;
    }
    public bool IsInteractable() {
        return interactable; 
    }

    protected virtual void OnDestroy() {
        InteractableManager.Instance.RemoveInteractable(this);
    }
}
