using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorbalanEyeColor : MonoBehaviour {
    // components
    public Material eyesMaterial;
    
    // shared state
    public FloatReference torbalanAwareness;
    
    // state
    public float baseIntensity;
    public float flashIntensity;

    private const string EMISSIVE = "_EmissionColor";

    // Update is called once per frame
    void Update()
    {
        Color color;
        if (torbalanAwareness.Value >= 1) {
            color = Color.red;
        }
        else if (torbalanAwareness.Value >= 0.5f) {
            color = Color.red;
        }
        else if (torbalanAwareness.Value > 0) {
            float t = torbalanAwareness.Value * 2f;
            color = Color.Lerp(Color.yellow, Color.red, t);
        }
        else {
            color = Color.yellow;
        }

        Color emissiveColor = color * Mathf.GammaToLinearSpace(baseIntensity);
        eyesMaterial.SetColor(EMISSIVE, emissiveColor);
    }
}