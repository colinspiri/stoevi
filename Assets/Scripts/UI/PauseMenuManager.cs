using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour {
    public static PauseMenuManager Instance;
    
    [SerializeField] GameObject pauseMenu;
    private InputActions inputActions;
    
    // components
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverMessage;
    public TextMeshProUGUI gameOverTomatoes;
    public List<GameObject> otherUIObjects;

    private void Awake() {
        Instance = this;
    }

    private void Start()
    {
        inputActions = new InputActions();
        inputActions.Enable();
        
        gameOverPanel.SetActive(false);
        ClosePauseMenu();
    }

    private void Update()
    {
        if (!gameOverPanel.activeSelf && (GameManager.Instance == null || !GameManager.Instance.gameStopped)) {
            if (inputActions.Gameplay.Menu.triggered) {
                OpenPauseMenu();
            }
        }
    }

    private void OpenPauseMenu() {
        pauseMenu.SetActive(true);
        
        // disable other UI
        foreach (var uiObject in otherUIObjects) {
            uiObject.SetActive(false);
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if(GameManager.Instance) GameManager.Instance.Pause(true);
    }

    public void ClosePauseMenu() {
        pauseMenu.SetActive(false);
        
        // enable other UI
        foreach (var uiObject in otherUIObjects) {
            uiObject.SetActive(true);
        }
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if(GameManager.Instance) GameManager.Instance.Resume(true);
    }

    public void GameOver(bool playerSurvived = true) {
        gameOverPanel.SetActive(true);
        if (playerSurvived) {
            gameOverPanel.GetComponent<Image>().color = Color.black;
            gameOverMessage.text = "You survived day " + (GameManager.Instance.currentDay - 1) + ".";
            
            gameOverTomatoes.text = "You harvested " + GameManager.Instance.PlayerTomatoes + " tomatoes.\n" +
                                    "The Torbalan stole " + GameManager.Instance.TorbalanTomatoes + " tomatoes.";
        }
        else {
            gameOverMessage.text = "You failed.";
            
            gameOverTomatoes.text = "The Torbalan stole " + GameManager.Instance.TorbalanTomatoes + " tomatoes.\n" +
                                    "He also stole the " + GameManager.Instance.PlayerTomatoes + " you harvested.";
        }
    }
}