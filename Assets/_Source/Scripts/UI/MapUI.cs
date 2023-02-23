using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapUI : MonoBehaviour {
    // components
    private InputActions inputActions;
    public CanvasGroup canvasGroup;
    
    // state
    private bool mapEnabled;

    private void Awake() {
        inputActions = new InputActions();
        inputActions.Enable();
    }

    // Start is called before the first frame update
    void Start() {
        SetMapEnabled(false);

        inputActions.Gameplay.Map.performed += ToggleMap;
    }

    public void SetMapEnabled(bool value) {
        mapEnabled = value;

        canvasGroup.alpha = mapEnabled ? 1 : 0;
    }

    private void ToggleMap(InputAction.CallbackContext context) {
        SetMapEnabled(!mapEnabled);
    }

    private void OnDestroy() {
        inputActions.Gameplay.Map.performed -= ToggleMap;
    }
}
