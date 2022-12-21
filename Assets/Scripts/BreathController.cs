using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BreathController : MonoBehaviour {
    // public variables
    public float breatheLoudness;
    public float exhaleLoudness;
    
    // state
    public bool holdingBreath { get; private set; }

    // Update is called once per frame
    void Update()
    {
        // consume stamina
        if (holdingBreath) {
            StaminaController.Instance.ConsumeStamina();
        }
        
        // report sound
        if (TorbalanSenses.Instance != null && !holdingBreath) {
            TorbalanSenses.Instance.ReportSound(transform.position, breatheLoudness);
        }
    }

    private void StartHoldingBreath() {
        holdingBreath = true;
        
        // breathe in SFX
    }

    private void StopHoldingBreath() {
        holdingBreath = false;
        
        // breathe out SFX
        
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
