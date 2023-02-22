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
    private InputActions inputActions;
    public HeldItem heldItem;

    // all interactables
    private List<Crop> allCrops = new List<Crop>();
    private List<Soil> allSoil = new List<Soil>();
    
    // state
    public enum InteractionState { None, Selecting, Interacting }
    public InteractionState interactionState { get; private set; }
    public Interactable selectedObject { get; private set; }
    private float interactingTimer;

    private void Awake() {
        Instance = this;
        
        inputActions = new InputActions();
        inputActions.Enable();
        inputActions.Gameplay.Drop.performed += context => {
            heldItem.DropItem();
        };
    }

    private void Start() {
        interactionState = InteractionState.None;
        selectedObject = null;

        InputHandler.OnInteractPressed += () => {
            if (interactionState == InteractionState.Selecting) {
                // check if interactable
                if (!selectedObject.IsInteractable()) return;
                
                // tap to interact
                if(selectedObject.InteractionTime == 0) selectedObject.Interact();
                // hold to interact
                else StartInteracting();
            }
            /*else if (interactionState == InteractionState.None) {
                // drop item
                if (heldItem.HoldingItem()) {
                    heldItem.DropItem();
                }
            }*/
        };
    }

    private void Update() {
        // look for object to select
        LookForObjectToSelect();
        
        // selecting object
        if (interactionState == InteractionState.Selecting) {
            
        }
        // interacting with object
        else if (interactionState == InteractionState.Interacting) {
            // if no longer interactable or button released, stop interacting
            if (!selectedObject.IsInteractable() || !InputHandler.Instance.interact) {
                StopInteracting();
            }
            
            // count up hold timer
            interactingTimer += Time.deltaTime;
            
            // if holding for long enough, call interact
            if (interactingTimer >= selectedObject.InteractionTime) {
                selectedObject.Interact();
                StopInteracting();
                InputHandler.Instance.ResetInteractInput();
            }
        }
    }

    private void LookForObjectToSelect() {
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
    }

    private void Deselect() {
        selectedObject = null;
        interactionState = InteractionState.None;
        interactingTimer = 0;
    }

    private void SelectObject(Interactable interactable) {
        selectedObject = interactable;
        interactionState = InteractionState.Selecting;
        interactingTimer = 0;
    }

    private void StartInteracting() {
        interactionState = InteractionState.Interacting;
        selectedObject.OnStartInteracting();
        interactingTimer = 0;
    }

    private void StopInteracting() {
        interactionState = InteractionState.Selecting;
        selectedObject.OnStopInteracting();
        interactingTimer = 0;
    }

    public List<Crop> GetAllCrops() {
        return allCrops;
    }

    public Crop GetClosestCrop(List<Crop.GrowthStage> growthStages, Vector3 position, float maxDistance = float.MaxValue) {
        float closestDistance = float.MaxValue;
        Crop closestCrop = null;
        foreach (var crop in allCrops) {
            bool rightStage = false;
            foreach (var growthStage in growthStages) {
                if (crop.stage == growthStage) {
                    rightStage = true;
                    break;
                }
            }
            if (!rightStage) return null;
            
            float distance = Vector3.Distance(position, crop.transform.position);
            if (distance < closestDistance && distance < maxDistance) {
                closestDistance = distance;
                closestCrop = crop;
            }
        }

        return closestCrop;
    }
    
    public void AddInteractable(Interactable interactable) {
        if(interactable is Crop crop) allCrops.Add(crop);
        if(interactable is Soil soil) allSoil.Add(soil);
    }
    public void RemoveInteractable(Interactable interactable) {
        if (interactable is Crop crop) allCrops.Remove(crop);
        if (interactable is Soil soil) allSoil.Remove(soil);
        // if selected
        if (selectedObject == interactable) Deselect();
    }

    public void SaveAllData() {
        foreach (var crop in allCrops) {
            crop.AdvanceToNextDay();
        }
        foreach (var soil in allSoil) {
            soil.SaveData();
        }
    }

    public float GetInteractingFloat() {
        return interactingTimer / selectedObject.InteractionTime;
    }
}
