using System;
using System.Collections;
using UnityEngine;

public class TorbalanEyeColor : MonoBehaviour {
    // components
    public Material eyesMaterial;
    public FloatReference torbalanAwareness;
    [Space]

    // public variables
    public float flashBrightenTime;
    public float flashRemainTime;
    public float flashDimTime;
    [Header("Idle")]
    public float idleIntensity;
    [Space]
    // searching
    [Header("Searching")]
    public float searchIntensity;
    public float searchFlashIntensity;
    [Space]
    // chasing
    [Header("Chasing")]
    public float chaseIntensity;
    public float chaseFlashIntensity;
    
    // state
    private Color currentColor;
    private float currentIntensity;

    private const string EMISSIVE = "_EmissionColor";

    // Update is called once per frame
    void Update() {
        TorbalanStateTracker.TorbalanState state = TorbalanStateTracker.Instance.currentState;
        if (state == TorbalanStateTracker.TorbalanState.Chase) {
            currentColor = Color.red;
            currentIntensity = chaseIntensity;
        }
        else if (state == TorbalanStateTracker.TorbalanState.Search) {
            float t = torbalanAwareness.Value;
            currentColor = Color.Lerp(Color.yellow, Color.red, t);
            currentIntensity = searchIntensity;
        }
        else {
            currentColor = Color.yellow;
            currentIntensity = idleIntensity;
        }

        // update material
        Color emissiveColor = currentColor * Mathf.GammaToLinearSpace(currentIntensity);
        eyesMaterial.SetColor(EMISSIVE, emissiveColor);
    }

    /*public void FlashSearch() {
        currentColor = searchColor;
        StartCoroutine(FlashCoroutine(searchIntensity, searchFlashIntensity, EyeState.Search));
    }
    
    public void FlashChase() {
        currentColor = chaseColor;
        StartCoroutine(FlashCoroutine(chaseIntensity, chaseFlashIntensity, EyeState.Chase));
    }

    private IEnumerator FlashCoroutine(float startIntensity, float flashIntensity, EyeState stateOnEnd) {
        state = EyeState.Flashing;
        currentIntensity = startIntensity;

        for (float t = 0; t < flashBrightenTime; ) {
            currentIntensity = Mathf.Lerp(startIntensity, flashIntensity, t / flashBrightenTime);
            
            t += Time.deltaTime;
            yield return null;
        }

        currentIntensity = flashIntensity;
        yield return new WaitForSeconds(flashRemainTime);
        
        for (float t = 0; t < flashDimTime; ) {
            currentIntensity = Mathf.Lerp(flashIntensity, startIntensity, t / flashDimTime);
            
            t += Time.deltaTime;
            yield return null;
        }

        currentIntensity = startIntensity;
        state = stateOnEnd;

        yield return null;
    }*/
}