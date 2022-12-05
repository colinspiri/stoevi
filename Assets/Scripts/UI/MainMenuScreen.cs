using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuScreen : MonoBehaviour {
    public EventSystem eventSystem;
    public Button playButton;
    public Button continueButton;
    public Button newGameButton;

    private void OnEnable() {
        playButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        newGameButton.gameObject.SetActive(false);
        
        if (PlayerPrefs.GetInt("CurrentDay", 1) == 1) {
            eventSystem.firstSelectedGameObject = playButton.gameObject;
            playButton.gameObject.SetActive(true);
        }
        else {
            eventSystem.firstSelectedGameObject = continueButton.gameObject;
            continueButton.gameObject.SetActive(true);
            newGameButton.gameObject.SetActive(true);
        }
    }
}
