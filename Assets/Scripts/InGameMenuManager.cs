using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenuManager : MonoBehaviour {
    public static InGameMenuManager Instance;
    
    
    
    private void Awake() {
        Instance = this;
    }

    private void Update() {
        /*if (inputActions.UI.Cancel.triggered) {
            if(menu.activeSelf) HideMenu();
            else if(!gameOverPanel.activeSelf) ShowMenu();
        }*/
    }
}
