using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursorState : MonoBehaviour {
    public bool setOnEnable;
    public bool cursorVisibleOnEnable;
    public CursorLockMode cursorStateOnEnable;

    public bool setOnDisable;
    public bool cursorVisibleOnDisable;
    public CursorLockMode cursorStateOnDisable;

    private void OnEnable() {
        if (setOnEnable) {
            Cursor.lockState = cursorStateOnEnable;
            Cursor.visible = cursorVisibleOnEnable;
        }
    }

    private void OnDisable() {
        if (setOnDisable) {
            Cursor.lockState = cursorStateOnDisable;
            Cursor.visible = cursorVisibleOnDisable;
        }
    }
}
