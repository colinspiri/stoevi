using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour {
    // components
    private InputActions inputActions;
    public CanvasGroup canvasGroup;
    
    // state
    private bool mapEnabled;

    private void Awake() {
        inputActions = new InputActions();
        inputActions.Enable();
        inputActions.Gameplay.Map.performed += context => {
            SetMapEnabled(!mapEnabled);
        };
    }

    // Start is called before the first frame update
    void Start() {
        SetMapEnabled(false);
    }

    public void SetMapEnabled(bool value) {
        mapEnabled = value;

        canvasGroup.alpha = mapEnabled ? 1 : 0;
    }
}
