using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Interactable : MonoBehaviour {
    // constants
    [FormerlySerializedAs("interactionDistance")] public float InteractionDistance = 5f;
    [FormerlySerializedAs("interactionTime")] public float InteractionTime = 1f;

    // Start is called before the first frame update
    protected virtual void Start() {
        InteractableManager.Instance.AddInteractable(this);
    }

    public abstract void Interact();

    public abstract string GetObjectName();
    public abstract string GetObjectDescription();
    public abstract string GetButtonPrompt();
    
    public virtual float GetTimerValue() { return 0; }
    public virtual float GetTimerTime() { return 0; }
    public virtual InteractableUI.TimerIcon GetTimerIcon() { return InteractableUI.TimerIcon.None; }

    public bool PlayerWithinInteractionDistance(Vector3 raycastPoint) {
        var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, raycastPoint);
        return distance <= InteractionDistance;
    }

    public virtual bool IsSelectable() {
        return true;
    }
    public virtual bool IsInteractable() {
        return true;
    }

    protected virtual void OnDestroy() {
        InteractableManager.Instance.RemoveInteractable(this);
    }
}
