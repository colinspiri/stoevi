using System.Collections.Generic;
using UnityEngine;

public class InteractableManager : MonoBehaviour {
    // components
    public static InteractableManager Instance;

    // all interactables
    private List<Crop> allCrops = new List<Crop>();
    private List<Soil> allSoil = new List<Soil>();
    
    // state
    public enum InteractionState { None, Selecting, Interacting }
    public InteractionState interactionState { get; private set; }
    public Interactable selectedObject { get; private set; }
    private enum InteractType { Primary, Secondary }
    private InteractType currentInteractType;
    private float interactingTimer;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        interactionState = InteractionState.None;
        selectedObject = null;

        InputHandler.OnInteractPrimaryPressed += () => {
            if (interactionState == InteractionState.Selecting) {
                // check if interactable
                if (!selectedObject.IsInteractablePrimary()) return;
                
                // tap to interact
                if(selectedObject.InteractionTimePrimary == 0) selectedObject.InteractPrimary();
                // hold to interact
                else StartInteracting(InteractType.Primary);
            }
        };
        InputHandler.OnInteractSecondaryPressed += () => {
            if (interactionState == InteractionState.Selecting) {
                // check if interactable
                if (!selectedObject.IsInteractableSecondary()) return;
                
                // tap to interact
                if (selectedObject.InteractionTimeSecondary == 0) {
                    selectedObject.InteractSecondary();
                }
                // hold to interact
                else StartInteracting(InteractType.Secondary);
            }
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
            if (currentInteractType == InteractType.Primary && (!selectedObject.IsInteractablePrimary() || !InputHandler.Instance.interactPrimary)) {
                StopInteracting(InteractType.Primary);
            }
            if (currentInteractType == InteractType.Secondary && (!selectedObject.IsInteractableSecondary() || !InputHandler.Instance.interactSecondary)) {
                StopInteracting(InteractType.Secondary);
            }
            
            // count up hold timer
            interactingTimer += Time.deltaTime;
            
            // if holding for long enough, call interact
            float interactionTime = currentInteractType == InteractType.Primary
                ? selectedObject.InteractionTimePrimary
                : selectedObject.InteractionTimeSecondary;
            if (interactingTimer >= interactionTime) {
                if(currentInteractType == InteractType.Primary) selectedObject.InteractPrimary();
                else if(currentInteractType == InteractType.Secondary) selectedObject.InteractSecondary();
                
                StopInteracting(currentInteractType);
                InputHandler.Instance.ResetInteractInput();
            }
        }
    }

    private void LookForObjectToSelect() {
        // if game is stopped
        if (GameManager.Instance && GameManager.Instance.gameStopped) return;
        
        // see if camera is pointing at an object
        RaycastHit hitInfo;
        if (CameraRaycast.Instance == null) return;
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

    private void StartInteracting(InteractType interactType) {
        interactionState = InteractionState.Interacting;
        interactingTimer = 0;
        currentInteractType = interactType;

        if (interactType == InteractType.Primary) {
            selectedObject.OnStartInteractingPrimary();
        }
        else if (interactType == InteractType.Secondary) {
            selectedObject.OnStartInteractingSecondary();
        }
    }

    private void StopInteracting(InteractType interactType) {
        interactionState = InteractionState.Selecting;
        interactingTimer = 0;

        if (interactType == InteractType.Primary) {
            selectedObject.OnStopInteractingPrimary();
        }
        else if (interactType == InteractType.Secondary) {
            selectedObject.OnStopInteractingSecondary();
        }
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
        if (currentInteractType == InteractType.Primary) {
            return interactingTimer / selectedObject.InteractionTimePrimary;
        }
        else if (currentInteractType == InteractType.Secondary) {
            return interactingTimer / selectedObject.InteractionTimeSecondary;
        }
        return 0;
    }
}
