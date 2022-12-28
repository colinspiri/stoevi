using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour {
    public static InputHandler Instance;
    
    [Header("Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool run;
    
    public bool crouching;
    private bool crouchingLastFrame;
    
    public bool peek;
    
    public bool interact;
    public static event Action OnInteractPressed = delegate { };
    
    public bool holdBreath;
    public static event Action<bool> OnHoldBreathPressed = delegate {  };
    
    public static event Action OnDebugGameOverPressed = delegate {  };

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;
    public float sensitivity = 1;

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        // toggle crouching
        if(crouching && !crouchingLastFrame) {
            crouchingLastFrame = true;
        }
        if(crouchingLastFrame && !crouching) {
            crouchingLastFrame = false;
        }
    }

    public void OnMove(InputValue value) {
        move = value.Get<Vector2>();
    }
    public void OnLook(InputValue value) {
        if(cursorInputForLook) {
            look = sensitivity * value.Get<Vector2>();
        }
    }
    public void OnRun(InputValue value) {
        run = value.isPressed;
    }
    public void OnCrouch(InputValue value) {
        if (!crouchingLastFrame && value.isPressed) {
            crouching = true;
        }
        if (crouchingLastFrame && value.isPressed) {
            crouching = false;
        }
    }
    public void OnPeek(InputValue value) {
        peek = value.isPressed;
    }
    
    public void OnInteract(InputValue value) {
        interact = value.isPressed;
        
        if (interact) OnInteractPressed();
    }
    public void ResetInteractInput() {
        interact = false;
    }
    
    public void OnHoldBreath(InputValue value) {
        holdBreath = value.isPressed;
        
        OnHoldBreathPressed(holdBreath);
    }

    public void OnDebugGameOver(InputValue value) {
        OnDebugGameOverPressed();
    }
    
    private void OnApplicationFocus(bool hasFocus) {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState) {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}