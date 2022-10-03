using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TomatoCounter : MonoBehaviour
{
    public static TomatoCounter Instance;
    
    // components
    public TextMeshProUGUI counterText;
    
    // state
    private int playerTomatoes;
    public int PlayerTomatoes => playerTomatoes;
    private int torbalanTomatoes;
    public int TorbalanTomatoes => torbalanTomatoes;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        counterText.text = "harvested 0 tomatoes";
    }

    public void PlayerHarvestedTomato() {
        playerTomatoes++;
        counterText.text = "harvested " + playerTomatoes + " tomatoes";
    }

    public void TorbalanStoleTomato() {
        torbalanTomatoes++;
    }
}
