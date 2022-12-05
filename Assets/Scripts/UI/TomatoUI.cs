using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TomatoUI : MonoBehaviour
{
    public static TomatoUI Instance;
    
    // components
    public TextMeshProUGUI counterText;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        counterText.text = "harvested 0 tomatoes";
        if(GameManager.Instance) GameManager.Instance.playerHarvestedTomato.AddListener(tomatoes => {
            counterText.text = "harvested " + tomatoes + " tomatoes";
        });
    }
}
