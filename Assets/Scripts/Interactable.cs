using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour {
    // constants
    public float interactionDistance = 5f;
    
    // state
    private bool selected;
    private bool interactable = true;

    // Start is called before the first frame update
    protected virtual void Start() {
        InteractableManager.Instance.AddInteractable(this);
    }
    
    public virtual void Select() {
        selected = true;
    }

    public virtual void Deselect() {
        selected = false;
    }

    public abstract void Interact();

    public abstract string GetUIText();

    public virtual float GetSliderFloat() {
        return 0; 
    }
    
    

    public bool PlayerWithinInteractionDistance(Vector3 raycastPoint) {
        var distance = Vector3.Distance(FirstPersonController.Instance.transform.position, raycastPoint);
        return distance <= interactionDistance;
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
