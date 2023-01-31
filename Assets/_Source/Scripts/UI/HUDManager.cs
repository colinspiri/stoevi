using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour {
    // components
    private InputActions inputActions;
    public CanvasGroup canvasGroup;

    // state
    private bool hudEnabled;

    private void Awake() {
        inputActions = new InputActions();
        inputActions.Enable();
        inputActions.Gameplay.ToggleHUD.performed += context => {
            SetHUDVisible(!hudEnabled);
        };
    }

    // Start is called before the first frame update
    void Start() {
        SetHUDVisible(true);
    }

    public void SetHUDVisible(bool value) {
        hudEnabled = value;

        canvasGroup.alpha = hudEnabled ? 1 : 0;
    }
}
