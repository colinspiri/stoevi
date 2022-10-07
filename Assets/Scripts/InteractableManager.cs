using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractableManager : MonoBehaviour {
    // components
    public static InteractableManager Instance;
    
    // public constants
    public float interactionRadius = 5.0f;
    public float interactionAngle = 80.0f;

    // all interactables
    private List<Interactable> allInteractables = new List<Interactable>();
    private List<Crop> allCrops = new List<Crop>();
    // state
    private List<Interactable> candidatesForInteraction = new List<Interactable>();
    private Interactable selectedObject;
    public UnityEvent<Interactable> onSelectedObjectChange;

    private void Awake() {
        Instance = this;
    }

    // Update is called once per frame
    void Update() {
        SelectObjectFromCandidates();
    }

    public void CheckForHarvestableCropsLeft() {
        foreach (var crop in allCrops) {
            if (crop.cropState != Crop.CropState.Empty) return;
        }
        InGameMenuManager.Instance.GameOver(true);
    }

    private void SelectObjectFromCandidates() {
        // no candidates
        if (candidatesForInteraction.Count == 0) {
            if(selectedObject != null) selectedObject.Deselect();
            selectedObject = null;
            onSelectedObjectChange?.Invoke(selectedObject);
            // interactableUI.ClearInteractableUI();
            return;
        }
        // 1 candidate
        if (candidatesForInteraction.Count == 1) {
            if(selectedObject != null) selectedObject.Deselect();
            selectedObject = candidatesForInteraction[0];
            selectedObject.Select();
            onSelectedObjectChange?.Invoke(selectedObject);
            // interactableUI.ShowSelectedObject(selectedObject);
            return;
        }
        // choose closest candidate
        float minDistance = float.MaxValue;
        Interactable closest = null;
        foreach (Interactable candidate in candidatesForInteraction) {
            if (candidate.GetDistanceToPlayer() < minDistance){
                minDistance = candidate.GetDistanceToPlayer();
                closest = candidate;
            }
        }
        if(selectedObject != null) selectedObject.Deselect();
        selectedObject = closest;
        selectedObject.Select();
        onSelectedObjectChange?.Invoke(selectedObject);
        // interactableUI.ShowSelectedObject(selectedObject);
    }

    public Crop GetClosestHarvestableCropTo(Vector3 position, float maxDistance = float.MaxValue) {
        float closestDistance = float.MaxValue;
        Crop closestCrop = null;
        foreach (var crop in allCrops) {
            if (crop.cropState != Crop.CropState.Harvest) continue;
            float distance = Vector3.Distance(position, crop.transform.position);
            if (distance < closestDistance && distance < maxDistance) {
                closestDistance = distance;
                closestCrop = crop;
            }
        }

        return closestCrop;
    }

    public void AddCandidate(Interactable interactable) {
        if(!candidatesForInteraction.Contains(interactable)) candidatesForInteraction.Add(interactable);
    }
    public void RemoveCandidate(Interactable interactable) {
        if (candidatesForInteraction.Contains(interactable)) candidatesForInteraction.Remove(interactable);
    }

    public void AddInteractable(Interactable interactable) {
        allInteractables.Add(interactable);
        if(interactable is Crop crop) allCrops.Add(crop);
    }
    public void RemoveInteractable(Interactable interactable) {
        allInteractables.Remove(interactable);
        if (candidatesForInteraction.Contains(interactable)) candidatesForInteraction.Remove(interactable);
        if (interactable is Crop crop) allCrops.Remove(crop);
    }
    
    public void OnInteractInput(InputAction.CallbackContext context) {
        if (selectedObject == null) return;
        if(selectedObject.IsInteractable()) selectedObject.Interact();
    }
}
