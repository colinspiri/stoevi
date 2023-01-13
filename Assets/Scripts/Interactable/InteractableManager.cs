using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InteractableManager : MonoBehaviour {
    // components
    public static InteractableManager Instance;
    public InputActions inputActions;

    // all interactables
    private List<Interactable> allInteractables = new List<Interactable>();
    private List<Crop> allCrops = new List<Crop>();
    private List<Soil> allSoil = new List<Soil>();
    
    // state
    public enum InteractionState { None, Selecting, Interacting }
    public InteractionState interactionState { get; private set; }
    public Interactable selectedObject { get; private set; }
    private float holdTimer;

    private void Awake() {
        Instance = this;
        
        inputActions = new InputActions();
        inputActions.Enable();
    }

    private void Start() {
        interactionState = InteractionState.None;
        selectedObject = null;

        InputHandler.OnInteractPressed += () => {
            if (interactionState == InteractionState.None && ResourceManager.Instance.carryingFertilizer) {
                ResourceManager.Instance.DropFertilizer();
            }
        };
    }

    private void Update() {
        // receive input
        if (InputHandler.Instance.interact) {
            if (interactionState == InteractionState.Selecting && selectedObject.IsInteractable()) {
                StartInteracting();
            }
        }
        else if (interactionState == InteractionState.Interacting) {
            StopInteracting();
        }
        
        // if game is stopped
        if (GameManager.Instance && GameManager.Instance.gameStopped) return;
        
        // see if camera is pointing at an object
        RaycastHit hitInfo;
        Interactable interactable = CameraRaycast.Instance.GetCurrentInteractable(out hitInfo);
        
        // if no object, deselect
        if (interactable == null) {
            Deselect();
            return;
        }
        
        // check if within distance
        if (!interactable.PlayerWithinInteractionDistance(hitInfo.point)) {
            Deselect();
            return;
        }

        // check if selectable
        if (!interactable.IsSelectable()) {
            Deselect();
            return;
        }

        // if not already selected, select it
        if (interactable != selectedObject) SelectObject(interactable);
        
        // count up hold timer while interacting
        if (interactionState == InteractionState.Interacting) {
            // if no longer interactable, stop interacting
            if (!selectedObject.IsInteractable()) {
                StopInteracting();
            }
            
            // count up hold timer
            holdTimer += Time.deltaTime;
            
            // if holding for long enough, call interact
            if (holdTimer >= selectedObject.InteractionTime) {
                selectedObject.Interact();
                StopInteracting();
                InputHandler.Instance.ResetInteractInput();
            }
        }
    }

    private void Deselect() {
        selectedObject = null;
        interactionState = InteractionState.None;
        holdTimer = 0;
    }

    private void SelectObject(Interactable interactable) {
        selectedObject = interactable;
        interactionState = InteractionState.Selecting;
        holdTimer = 0;
    }

    private void StartInteracting() {
        interactionState = InteractionState.Interacting;
        selectedObject.OnStartInteracting();
        holdTimer = 0;
    }

    private void StopInteracting() {
        interactionState = InteractionState.Selecting;
        selectedObject.OnStopInteracting();
        holdTimer = 0;
    }

    public List<Crop> GetAllCrops() {
        return allCrops;
    }

    public Crop GetClosestHarvestableCropTo(Vector3 position, float maxDistance = float.MaxValue) {
        float closestDistance = float.MaxValue;
        Crop closestCrop = null;
        foreach (var crop in allCrops) {
            if (crop.stage != Crop.GrowthStage.Ripe) continue;
            float distance = Vector3.Distance(position, crop.transform.position);
            if (distance < closestDistance && distance < maxDistance) {
                closestDistance = distance;
                closestCrop = crop;
            }
        }

        return closestCrop;
    }

    public void SaveAllData() {
        foreach (var crop in allCrops) {
            crop.AdvanceToNextDay();
        }
        foreach (var soil in allSoil) {
            soil.SaveData();
        }
    }

    public void AddInteractable(Interactable interactable) {
        allInteractables.Add(interactable);
        if(interactable is Crop crop) allCrops.Add(crop);
        if(interactable is Soil soil) allSoil.Add(soil);
    }
    public void RemoveInteractable(Interactable interactable) {
        allInteractables.Remove(interactable);
        if (interactable is Crop crop) allCrops.Remove(crop);
        if (interactable is Soil soil) allSoil.Remove(soil);
        // if selected
        if (selectedObject == interactable) Deselect();
    }

    public float GetInteractingFloat() {
        return holdTimer / selectedObject.InteractionTime;
    }
}
