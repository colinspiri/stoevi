using System;
using UnityEngine;
using Yarn.Unity;

public class ClickToContinue : MonoBehaviour {
    private InputActions inputActions;
    public LineView lineView;

    private void Awake() {
        inputActions = new InputActions();
    }

    private void OnEnable() {
        inputActions.Enable();
        inputActions.UI.Click.performed += context => {
            Debug.Log("continue clicked " + context);
            lineView.OnContinueClicked();
        };
    }
    private void OnDisable() {
        inputActions.Disable();
    }
}
