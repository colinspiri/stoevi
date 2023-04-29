using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ClickToCallEvent : MonoBehaviour
{
    private InputActions inputActions;
    public UnityEvent onClick;

    private void Awake() {
        inputActions = new InputActions();
    }

    private void OnEnable() {
        inputActions.Enable();
    }
    private void OnDisable() {
        inputActions.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputActions.UI.Click.ReadValue<float>() > 0) {
            onClick.Invoke();
        }
    }
}
