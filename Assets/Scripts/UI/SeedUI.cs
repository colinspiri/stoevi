using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SeedUI : MonoBehaviour
{
    // components
    public TextMeshProUGUI counterText;
    
    private void Start() {
        if (ResourceManager.Instance) {
            UpdateText(ResourceManager.Instance.CurrentSeeds);
            ResourceManager.Instance.onSeedsChange.AddListener(UpdateText);
        }
    }

    private void UpdateText(int value) {
        counterText.text = value + "/" + ResourceManager.Instance.maxSeeds;
    }
}