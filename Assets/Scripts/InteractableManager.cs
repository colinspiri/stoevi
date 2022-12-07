using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractableManager : MonoBehaviour {
    // components
    public static InteractableManager Instance;

    // all interactables
    private List<Interactable> allInteractables = new List<Interactable>();
    private List<Crop> allCrops = new List<Crop>();
    // state
    private Interactable selectedObject;
    public UnityEvent<Interactable> onSelectedObjectChange;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
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
        if (interactable.PlayerWithinInteractionDistance(hitInfo.point) && interactable.IsInteractable()) {
            SelectObject(interactable);
        }
        else SelectObject(null);
    }

    private void SelectObject(Interactable interactable) {
        // if interactable is already selected, return
        if (interactable == selectedObject) return;

        // deselect old object
        if (selectedObject != null) {
            selectedObject.Deselect();
        }

        selectedObject = interactable;
        
        // select new object
        if (selectedObject != null) {
            selectedObject.Select();
        }
        
        onSelectedObjectChange?.Invoke(selectedObject);
    }
    
    
    public Interactable GetSelectedObject() {
        return selectedObject;
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
        if (context.performed) {
            if (selectedObject == null) return;
            if (selectedObject.IsInteractable()) selectedObject.Interact();
        }
    }
}
