using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HUDManager : MonoBehaviour {
    // components
    private InputActions inputActions;
    public CanvasGroup canvasGroup;

    // state
    private bool hudEnabled;

    private void Awake() {
        inputActions = new InputActions();
        inputActions.Enable();
    }

    // Start is called before the first frame update
    void Start() {
        SetHUDEnabled(true);

        inputActions.Gameplay.ToggleHUD.performed += ToggleHUD;
    }

    public void SetHUDEnabled(bool value) {
        hudEnabled = value;

        canvasGroup.alpha = hudEnabled ? 1 : 0;
    }

    private void ToggleHUD(InputAction.CallbackContext context) {
        SetHUDEnabled(!hudEnabled);
    }

    private void OnDestroy() {
        inputActions.Gameplay.ToggleHUD.performed -= ToggleHUD;
    }
}
