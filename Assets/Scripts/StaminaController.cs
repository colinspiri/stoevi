using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaController : MonoBehaviour {
    // components
    public static StaminaController Instance;

    // constants
    public float maxStamina;
    public float pauseTime;
    public float recoverThreshold;
    
    // state
    private enum State { Decreasing, Paused, Increasing, Recovering }
    private State state = State.Increasing;
    private float currentStamina;
    private float pauseTimer;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        currentStamina = maxStamina;
    }

    // Update is called once per frame
    void Update() {
        if (state != State.Recovering || currentStamina > recoverThreshold) {
            if(ConsumingStamina()) state = State.Decreasing;
            else if (state == State.Decreasing) {
                state = State.Paused;
                pauseTimer = pauseTime;
            }
        }
        
        if (state == State.Decreasing) {
            DecreaseStamina();
        }
        else if (state == State.Paused) {
            UpdatePaused();
        }
        else if (state == State.Increasing) {
            UpdateIncreasing();
        }
        else if (state == State.Recovering) {
           UpdateRecovering(); 
        }

        Debug.Log("state = " + state + " " + currentStamina);
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
            state = State.Recovering;
        }
    }

    private void UpdatePaused() {
        pauseTimer -= Time.deltaTime;
        Debug.Log("pause timer = " + pauseTimer);
        
        if (pauseTimer <= 0) {
            state = State.Increasing;
        }
    }

    private void UpdateIncreasing() {
        currentStamina += Time.deltaTime;
        
        if (currentStamina > maxStamina) currentStamina = maxStamina;
    }

    private void UpdateRecovering() {
        currentStamina += Time.deltaTime;
        
        if (currentStamina > maxStamina) {
            currentStamina = maxStamina;
            state = State.Increasing;
        }
    }

    public bool HasStamina() {
        return currentStamina > 0 && (state != State.Recovering || currentStamina > recoverThreshold);
    }
}
