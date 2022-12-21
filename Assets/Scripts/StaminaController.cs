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
    public enum StaminaState { Decreasing, Increasing, Recovering }
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
        if (staminaState == StaminaState.Decreasing) {
            pauseTimer -= Time.deltaTime;
        
            if (pauseTimer <= 0) {
                staminaState = StaminaState.Increasing;
            }
        }
        else if (staminaState == StaminaState.Increasing) {
            UpdateIncreasing();
        }
        else if (staminaState == StaminaState.Recovering) {
           UpdateRecovering();
        }
    }
    
    #region Public Functions
    public bool AtMaxStamina() {
        return currentStamina >= maxStamina;
    }

    public bool HasStamina() {
        return staminaState != StaminaState.Recovering && currentStamina > 0;
    }

    public void ConsumeStamina(float multiplier = 1.0f) {
        if (!HasStamina()) return;
        
        staminaState = StaminaState.Decreasing;
        pauseTimer = pauseTime;

        ChangeStamina(-multiplier * Time.deltaTime);
    }
    #endregion
    
    #region Helper Functions
    private void UpdateIncreasing() {
        if (currentStamina >= maxStamina) return;
        
        ChangeStamina(increaseRate * Time.deltaTime);
    }

    private void UpdateRecovering() {
        ChangeStamina(recoverRate * Time.deltaTime);
        
        if (currentStamina > recoverThreshold) {
            staminaState = StaminaState.Increasing;
        }
    }
    
    private void ChangeStamina(float amount) {
        currentStamina += amount;
        
        if (currentStamina < 0) {
            currentStamina = 0;
            staminaState = StaminaState.Recovering;
        }
        if (currentStamina > maxStamina) {
            currentStamina = maxStamina;
        }
        
        onStaminaChange?.Invoke(currentStamina/maxStamina);
    }
    
    #endregion
}
