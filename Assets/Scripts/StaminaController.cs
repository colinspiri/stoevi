using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StaminaController : MonoBehaviour {
    // components
    public static StaminaController Instance;

    // constants
    public float maxStamina;
    public float pauseTime;
    public float increaseRate;
    public float recoverRate;
    public float recoverThreshold;
    
    // state
    public enum StaminaState { Decreasing, Paused, Increasing, Recovering }
    public StaminaState staminaState { get; private set; }
    private float currentStamina;
    private float pauseTimer;
    
    // callbacks
    public UnityEvent<float> onStaminaChange;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        currentStamina = maxStamina;
        staminaState = StaminaState.Increasing;
    }

    // Update is called once per frame
    void Update() {
        if (staminaState != StaminaState.Recovering) {
            if(ConsumingStamina()) staminaState = StaminaState.Decreasing;
            else if (staminaState == StaminaState.Decreasing) {
                staminaState = StaminaState.Paused;
                pauseTimer = pauseTime;
            }
        }
        
        if (staminaState == StaminaState.Decreasing) {
            DecreaseStamina();
        }
        else if (staminaState == StaminaState.Paused) {
            UpdatePaused();
        }
        else if (staminaState == StaminaState.Increasing) {
            UpdateIncreasing();
        }
        else if (staminaState == StaminaState.Recovering) {
           UpdateRecovering();
        }
    }

    private bool ConsumingStamina() {
        if (FirstPersonController.Instance.GetMoveState == FirstPersonController.MoveState.Running) {
            return true;
        }

        return false;
    }

    private void DecreaseStamina() {
        currentStamina -= Time.deltaTime;

        if (currentStamina < 0) {
            currentStamina = 0;
            staminaState = StaminaState.Recovering;
        }
        
        onStaminaChange?.Invoke(currentStamina/maxStamina);
    }

    private void UpdatePaused() {
        pauseTimer -= Time.deltaTime;
        
        if (pauseTimer <= 0) {
            staminaState = StaminaState.Increasing;
        }
    }

    private void UpdateIncreasing() {
        if (currentStamina == maxStamina) return;
        
        currentStamina += increaseRate * Time.deltaTime;
        
        if (currentStamina > maxStamina) currentStamina = maxStamina;
        
        onStaminaChange?.Invoke(currentStamina/maxStamina);
    }

    private void UpdateRecovering() {
        currentStamina += recoverRate * Time.deltaTime;
        
        if (currentStamina > recoverThreshold) {
            staminaState = StaminaState.Increasing;
        }
        
        onStaminaChange?.Invoke(currentStamina/maxStamina);
    }

    public bool AtMaxStamina() {
        return currentStamina >= maxStamina;
    }

    public bool HasStamina() {
        return currentStamina > 0 && staminaState != StaminaState.Recovering;
    }
}
