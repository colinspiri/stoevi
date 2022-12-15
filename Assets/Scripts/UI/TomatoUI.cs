using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TomatoUI : MonoBehaviour
{
    // components
    public TextMeshProUGUI counterText;

    private void Start() {
        if (ResourceManager.Instance) {
            UpdateText(ResourceManager.Instance.PlayerTomatoes);
            ResourceManager.Instance.playerHarvestedTomato.AddListener(UpdateText);
        }
    }

    private void UpdateText(int value) {
        counterText.text = value.ToString();
    }
}
