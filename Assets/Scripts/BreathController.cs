using System;
using System.Collections;
using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;
using UnityEngine.InputSystem;

public class BreathController : MonoBehaviour {
    // components
    public ASoundContainer inhaleSound;
    public ASoundContainer exhaleSound;
    
    // public variables
    public float breatheLoudness;
    public float exhaleLoudness;
    
    // state
    public bool holdingBreath { get; private set; }

    // Update is called once per frame
    void Update() {
        // stop holding breath when stamina runs out
        if(!StaminaController.Instance.HasStamina() && holdingBreath) StopHoldingBreath();
        
        // consume stamina
        if (holdingBreath) {
            StaminaController.Instance.ConsumeStamina();
        }
        
        // play SFX
        // tired
        if (StaminaController.Instance.staminaState == StaminaController.StaminaState.Recovering || StaminaController.Instance.staminaState == StaminaController.StaminaState.Decreasing) {
            AudioManager.Instance.SetTiredBreathingSound(true);
            AudioManager.Instance.SetBreathingSound(false);
        }
        // holding breath
        else if (holdingBreath) {
            AudioManager.Instance.SetTiredBreathingSound(false);
            AudioManager.Instance.SetBreathingSound(false);
        }
        // breathing normally
        else {
            AudioManager.Instance.SetTiredBreathingSound(false);
            AudioManager.Instance.SetBreathingSound(true);
        }
        
        // report sound
        if (TorbalanSenses.Instance != null && !holdingBreath) {
            TorbalanSenses.Instance.ReportSound(transform.position, breatheLoudness);
        }
    }

    private void StartHoldingBreath() {
        holdingBreath = true;
        
        // inhale SFX
        inhaleSound.Play();
    }

    private void StopHoldingBreath() {
        holdingBreath = false;
        
        // exhale SFX
        exhaleSound.Play();
        
        // report sound
        if (TorbalanSenses.Instance != null) {
            TorbalanSenses.Instance.ReportSound(transform.position, exhaleLoudness);
        }
    }
    
    public void OnHoldBreathInput(InputAction.CallbackContext context) {
        var shouldBeHoldingBreath = StaminaController.Instance.HasStamina() && context.ReadValueAsButton();
        
        if(holdingBreath && !shouldBeHoldingBreath) StopHoldingBreath();
        else if(!holdingBreath && shouldBeHoldingBreath) StartHoldingBreath();
    }
}
