using System;
using UnityEngine;

public class StaminaController : MonoBehaviour {
    // components
    public static StaminaController Instance;

    // constants
    public float maxStamina;
    public float pauseTime;
    public float increaseRate;
    public float recoverThreshold;
    
    // state
    public enum StaminaState { Maximum, Decreasing, Increasing, Recovering }
    public StaminaState staminaState;
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
        staminaState = StaminaState.Maximum;
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
        else if (staminaState == StaminaState.Maximum) {
            AudioManager.Instance.SetBreathingSound(false);
            AudioManager.Instance.SetTiredBreathingSound(false);
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
    public void PauseStaminaRegeneration() {
        if (!HasStamina()) return;
        
        ChangeStaminaState(StaminaState.Decreasing);
        pauseTimer = pauseTime;
    }
    #endregion
    
    #region Helper Functions

    private void UpdateDecreasing() {
        pauseTimer -= Time.deltaTime;
        
        AudioManager.Instance.SetBreathingSound(false);
        AudioManager.Instance.SetTiredBreathingSound(false);

        if (pauseTimer <= 0) {
            ChangeStaminaState(StaminaState.Increasing);
        }
    }
    private void UpdateIncreasing() {
        if (currentStamina >= maxStamina) {
            ChangeStaminaState(StaminaState.Maximum);
            return;
        }
        
        AudioManager.Instance.SetBreathingSound(true);
        AudioManager.Instance.SetTiredBreathingSound(false);

        ChangeStamina(increaseRate * Time.deltaTime);
    }

    private void UpdateRecovering() {
        ChangeStamina(increaseRate * Time.deltaTime);
        
        AudioManager.Instance.SetBreathingSound(false);
        AudioManager.Instance.SetTiredBreathingSound(true);
        
        if (currentStamina > recoverThreshold * maxStamina) {
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
