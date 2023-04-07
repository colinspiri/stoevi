using UnityEngine;
using UnityEngine.Serialization;

public abstract class Interactable : MonoBehaviour {
    // constants
    [FormerlySerializedAs("interactionDistance")] public float InteractionDistance = 5f;
    [FormerlySerializedAs("InteractionTime")] [FormerlySerializedAs("interactionTime")] public float InteractionTimePrimary = 1f;
    public float InteractionTimeSecondary = 1f;

    protected virtual void Start() {
        if(InteractableManager.Instance != null) InteractableManager.Instance.AddInteractable(this);
    }

    public abstract void InteractPrimary();

    public virtual void InteractSecondary() {
        Debug.Log("InteractSecondary on " + gameObject.name);
    }

    public abstract string GetObjectName();
    public abstract string GetObjectDescription();
    public abstract string GetButtonPromptPrimary();

    public virtual string GetButtonPromptSecondary() {
        return "";
    }

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
    public virtual bool IsInteractablePrimary() {
        return true;
    }
    public virtual bool IsInteractableSecondary() {
        return false;
    }

    public virtual void OnStartInteractingPrimary() {
        
    }
    public virtual void OnStopInteractingPrimary() {
        
    }
    public virtual void OnStartInteractingSecondary() {
        
    }
    public virtual void OnStopInteractingSecondary() {
        
    }
    protected static string GetInteractPrimaryButton() {
        return "LMB";
    }
    protected static string GetInteractSecondaryButton() {
        return "RMB";
    }

    private void OnDestroy() {
        if(InteractableManager.Instance != null) InteractableManager.Instance.RemoveInteractable(this);
    }
}