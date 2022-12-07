using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractableManager : MonoBehaviour {
    // components
    public static InteractableManager Instance;

    // all interactables
    private List<Interactable> allInteractables = new List<Interactable>();
    private List<Crop> allCrops = new List<Crop>();
    
    // state
    public enum InteractionState { None, Selecting, Interacting }
    public InteractionState interactionState = InteractionState.None;
    public Interactable SelectedObject { get; private set; }
    
    private float holdTimer;
    
    private void Awake() {
        Instance = this;
    }

    private void Update() {
        if (GameManager.Instance && GameManager.Instance.gameStopped) return;
        
        // see if camera is pointing at an object
        RaycastHit hitInfo;
        var obj = CameraRaycast.Instance.GetCurrentObject(out hitInfo);
        if (obj == null) {
            SelectObject(null);
            return;
        }
        
        // check if object is interactable
        var interactable = obj.GetComponent<Interactable>();
        if (interactable == null) {
            SelectObject(null);
            return;
        }
        
        // if interactable is within distance and currently interactable, select it
        if (interactable.IsSelectable() && interactable.PlayerWithinInteractionDistance(hitInfo.point)) {
            SelectObject(interactable);
        }
        else SelectObject(null);


        // count up hold timer while interacting
        if (interactionState == InteractionState.Interacting) {
            // if no longer interactable, go back to selecting
            if (!SelectedObject.IsInteractable()) {
                holdTimer = 0;
                interactionState = InteractionState.Selecting;
            }
            // count up hold timer
            holdTimer += Time.deltaTime;
            // if holding for long enough, call interact
            if (holdTimer >= SelectedObject.InteractionTime) {
                SelectedObject.Interact();
            }
        }
    }

    private void SelectObject(Interactable interactable) {
        // check if same object
        if (SelectedObject == interactable) return;
        
        // select new object
        SelectedObject = interactable;
        
        // change state
        if (SelectedObject == null) interactionState = InteractionState.None;
        else {
            interactionState = InteractionState.Selecting;
        }

        // reset hold timer
        holdTimer = 0;
    }

    public void CheckForHarvestableCropsLeft() {
        foreach (var crop in allCrops) {
            if (crop.stage != Crop.CropStage.Bare) return;
        }
        GameManager.Instance.GameOver(true);
    }

    public Crop GetClosestHarvestableCropTo(Vector3 position, float maxDistance = float.MaxValue) {
        float closestDistance = float.MaxValue;
        Crop closestCrop = null;
        foreach (var crop in allCrops) {
            if (crop.stage != Crop.CropStage.Ripe) continue;
            float distance = Vector3.Distance(position, crop.transform.position);
            if (distance < closestDistance && distance < maxDistance) {
                closestDistance = distance;
                closestCrop = crop;
            }
        }

        return closestCrop;
    }

    public void AddInteractable(Interactable interactable) {
        allInteractables.Add(interactable);
        if(interactable is Crop crop) allCrops.Add(crop);
    }
    public void RemoveInteractable(Interactable interactable) {
        allInteractables.Remove(interactable);
        if (interactable is Crop crop) allCrops.Remove(crop);
    }
    
    public void OnInteractInput(InputAction.CallbackContext context) {
        if (context.ReadValueAsButton()) {
            if (interactionState == InteractionState.Selecting && SelectedObject.IsInteractable()) {
                interactionState = InteractionState.Interacting;
                holdTimer = 0;
            }
        }
        else {
            if (interactionState == InteractionState.Interacting) {
                interactionState = InteractionState.Selecting;
                holdTimer = 0;
            }
        }
    }

    public float GetInteractingFloat() {
        return holdTimer / SelectedObject.InteractionTime;
    }
}
