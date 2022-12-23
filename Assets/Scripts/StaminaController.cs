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
    public static event Action<float> OnStaminaChange = delegate { };
    public static event Action<StaminaState> OnStateChange = delegate { };

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
            UpdateDecreasing();
        }
        else if (staminaState == StaminaState.Increasing) {
            UpdateIncreasing();
        }
        else if (staminaState == StaminaState.Recovering) {
           UpdateRecovering();
        }
    }
    
    #region Public Functions
    public bool HasStamina() {
        return staminaState != StaminaState.Recovering && currentStamina > 0;
    }

    public void ConsumeStamina(float multiplier = 1.0f) {
        if (!HasStamina()) return;
        
        ChangeStaminaState(StaminaState.Decreasing);
        pauseTimer = pauseTime;

        ChangeStamina(-multiplier * Time.deltaTime);
    }
    #endregion
    
    #region Helper Functions

    private void UpdateDecreasing() {
        pauseTimer -= Time.deltaTime;
        
        if (pauseTimer <= 0) {
            ChangeStaminaState(StaminaState.Increasing);
        }
    }
    private void UpdateIncreasing() {
        if (currentStamina >= maxStamina) return;
        
        ChangeStamina(increaseRate * Time.deltaTime);
    }

    private void UpdateRecovering() {
        ChangeStamina(recoverRate * Time.deltaTime);
        
        if (currentStamina > recoverThreshold) {
            ChangeStaminaState(StaminaState.Increasing);
        }
    }

    private void ChangeStamina(float amount) {
        currentStamina += amount;
        
        if (currentStamina < 0) {
            currentStamina = 0;
            ChangeStaminaState(StaminaState.Recovering);
        }
        if (currentStamina >= maxStamina) {
            currentStamina = maxStamina;
        }

        OnStaminaChange(currentStamina / maxStamina);
    }
    
    private void ChangeStaminaState(StaminaState newState) {
        staminaState = newState;

        OnStateChange(staminaState);
    }
    
    #endregion
}
