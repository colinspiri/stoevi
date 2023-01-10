using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SeedUI : MonoBehaviour
{
    // components
    public TextMeshProUGUI counterText;
    public IntReference seeds;
    
    // state
    private int previousCount = -1;

    private void OnEnable() {
        UpdateText();
    }

    private void Update() {
        if (previousCount != seeds.Value) {
            UpdateText();
            previousCount = seeds.Value;
        }
    }

    private void UpdateText() {
        counterText.text = seeds.Value.ToString();
    }
}