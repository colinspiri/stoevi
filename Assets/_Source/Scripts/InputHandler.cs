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
    
    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;
    public FloatReference defaultSensitivity;
    public float sensitivity;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        // initialize sensitivity
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", defaultSensitivity);
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
        
        if (interact && !GameManager.Instance.gameStopped) OnInteractPressed();
    }
    public void ResetInteractInput() {
        interact = false;
    }
    
    public void OnHoldBreath(InputValue value) {
        holdBreath = value.isPressed;
        
        if(!GameManager.Instance.gameStopped) OnHoldBreathPressed(holdBreath);
    }

    public void OnToggleDebug(InputValue value) {
        DebugController.Instance.ToggleDebug();
    }

    public void OnReturn(InputValue value) {
        DebugController.Instance.Return();
    }
    
    private void OnApplicationFocus(bool hasFocus) {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState) {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}