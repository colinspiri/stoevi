using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorbalanEyeColor : MonoBehaviour {
    // components
    public Material eyesMaterial;
    
    // shared state
    public FloatReference torbalanAwareness;
    
    // constants
    public float minIntensity; // 20
    public float maxIntensity; // 80

    // Update is called once per frame
    void Update()
    {
        Color color;
        float intensity;
        if (torbalanAwareness.Value >= 1) {
            color = Color.red;
            intensity = maxIntensity;
        }
        else if (torbalanAwareness.Value >= 0.5f) {
            color = Color.red;
            float t = (torbalanAwareness.Value - 0.5f) * 2f;
            intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        }
        else if (torbalanAwareness.Value > 0) {
            float t = torbalanAwareness.Value * 2f;
            color = Color.Lerp(Color.yellow, Color.red, t);
            intensity = minIntensity;
        }
        else {
            color = Color.yellow;
            intensity = minIntensity;
        }
        eyesMaterial.SetColor("_EmissionColor", color * Mathf.GammaToLinearSpace(intensity));
    }
}