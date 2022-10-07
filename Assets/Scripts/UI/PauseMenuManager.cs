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

        pauseMenu.SetActive(false);
        ResumeGame();
        
        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (!pauseMenu.activeSelf && !gameOverPanel.activeSelf && inputActions.UI.Cancel.triggered)
        {
            OpenPauseMenu();
        }
    }

    private void OpenPauseMenu()
    {
        pauseMenu.SetActive(true);

        StopGame();
    }

    public void ClosePauseMenu()
    {
        pauseMenu.SetActive(false);

        ResumeGame();
    }

    private void StopGame()
    {
        // disable other UI
        foreach (var uiObject in otherUIObjects) {
            uiObject.SetActive(false);
        }
        
        Time.timeScale = 0.0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (AudioManager.Instance) AudioManager.Instance.StopGameSound();
        else Debug.LogError("Audio Manager not found");
    }

    private void ResumeGame()
    {
        // enable other UI
        foreach (var uiObject in otherUIObjects) {
            uiObject.SetActive(true);
        }
        
        Time.timeScale = 1.0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (AudioManager.Instance) AudioManager.Instance.ResumeGameSound();
        else Debug.LogError("Audio Manager not found");
    }
    
    public void GameOver(bool playerSurvived = true) {
        gameOverPanel.SetActive(true);
        if (playerSurvived) {
            gameOverPanel.GetComponent<Image>().color = Color.black;
            gameOverMessage.text = "You survived.";
            
            gameOverTomatoes.text = "You harvested " + TomatoCounter.Instance.PlayerTomatoes + " tomatoes.\n" +
                                    "The Torbalan stole " + TomatoCounter.Instance.TorbalanTomatoes + " tomatoes.";
        }
        else {
            gameOverMessage.text = "You failed.";
            
            gameOverTomatoes.text = "The Torbalan stole " + TomatoCounter.Instance.TorbalanTomatoes + " tomatoes.\n" +
                                    "He also stole the " + TomatoCounter.Instance.PlayerTomatoes + " you harvested.";
        }
        
        StopGame();
    }
}