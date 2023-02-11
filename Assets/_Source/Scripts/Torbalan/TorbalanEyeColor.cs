using System.Collections;
using UnityEngine;

public class TorbalanEyeColor : MonoBehaviour {
    // components
    public Material eyesMaterial;
    [Space]

    // public variables
    public float flashBrightenTime;
    public float flashRemainTime;
    public float flashDimTime;
    [Header("Idle")]
    public Color idleColor;
    public float idleIntensity;
    [Space]
    // searching
    [Header("Searching")]
    public Color searchColor;
    public float searchIntensity;
    public float searchFlashIntensity;
    [Space]
    // chasing
    [Header("Chasing")]
    public Color chaseColor;
    public float chaseIntensity;
    public float chaseFlashIntensity;
    
    // state
    private enum EyeState { Idle, Search, Chase, Flashing }
    private EyeState state;
    private Color currentColor;
    private float currentIntensity;

    private const string EMISSIVE = "_EmissionColor";

    private void Start() {
        state = EyeState.Idle;
    }

    // Update is called once per frame
    void Update() {
        // set current color and intensity
        if (state == EyeState.Idle) {
            currentColor = idleColor;
            currentIntensity = idleIntensity;
        }
        else if (state == EyeState.Search) {
            currentColor = searchColor;
            currentIntensity = searchIntensity;
        }
        else if (state == EyeState.Chase) {
            currentColor = chaseColor;
            currentIntensity = chaseIntensity;
        }

        // update material
        Color emissiveColor = currentColor * Mathf.GammaToLinearSpace(currentIntensity);
        eyesMaterial.SetColor(EMISSIVE, emissiveColor);
    }

    public void FlashSearch() {
        currentColor = searchColor;
        StartCoroutine(FlashCoroutine(searchIntensity, searchFlashIntensity, EyeState.Search));
    }
    
    public void FlashChase() {
        currentColor = chaseColor;
        StartCoroutine(FlashCoroutine(chaseIntensity, chaseFlashIntensity, EyeState.Chase));
    }

    public void EyesIdle() {
        state = EyeState.Idle;
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
    }
}