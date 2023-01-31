using SpookuleleAudio;
using UnityEngine;

public class BreathController : MonoBehaviour {
    // components
    public ASoundContainer inhaleSound;
    public ASoundContainer exhaleSound;
    
    // constants
    public FloatReference breatheLoudness;
    public FloatReference exhaleLoudness;
    
    // state
    public bool holdingBreath { get; private set; }
    
    // TODO: add passive breathing state with state transitions that triggers based on StaminaController.ChangeState

    private void Start() {
        InputHandler.OnHoldBreathPressed += value => {
            if(!holdingBreath && value && StaminaController.Instance.HasStamina()) StartHoldingBreath();
            if(holdingBreath && !value) StopHoldingBreath();
        };
    }

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
        if (TorbalanHearing.Instance != null && !holdingBreath) {
            TorbalanHearing.Instance.ReportSound(transform.position, breatheLoudness);
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
        if (TorbalanHearing.Instance != null) {
            TorbalanHearing.Instance.ReportSound(transform.position, exhaleLoudness);
        }
    }
}
