using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour {
    [SerializeField] private GameObject pauseMenu;
    private InputActions inputActions;
    
    // components
    public HUDManager hud;

    private void Start() {
        inputActions = new InputActions();
        inputActions.Enable();
        
        ClosePauseMenu();
    }

    private void Update() {
        if (GameManager.Instance != null && GameManager.Instance.gameStopped) return;
        if (inputActions.Gameplay.Menu.triggered) {
            OpenPauseMenu();
        }
    }

    private void OpenPauseMenu() {
        pauseMenu.SetActive(true);
        
        hud.SetHUDEnabled(false);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if(GameManager.Instance) GameManager.Instance.Pause(true);
    }

    public void ClosePauseMenu() {
        pauseMenu.SetActive(false);
        
        hud.SetHUDEnabled(true);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(GameManager.Instance) GameManager.Instance.Resume(true);
    }
}