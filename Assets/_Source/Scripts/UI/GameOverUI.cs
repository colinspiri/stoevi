using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
    // components
    private CanvasGroup canvasGroup;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverMessage;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start() {
        gameOverPanel.SetActive(false);
    }

    private void OnEnable() {
        GameManager.OnGameOver += ShowGameOverScreen;
    }

    private void OnDisable() {
        GameManager.OnGameOver -= ShowGameOverScreen;
    }

    private void ShowGameOverScreen(bool playerSurvived = true) {
        gameOverPanel.SetActive(true);
        
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.5f).SetUpdate(true);
        
        if (playerSurvived) {
            gameOverPanel.GetComponent<Image>().color = Color.black;
            gameOverMessage.text = "You survived day " + (GameManager.Instance.currentDay - 1) + ".";
        }
        else {
            gameOverMessage.text = "You died.";
        }
    }
}
