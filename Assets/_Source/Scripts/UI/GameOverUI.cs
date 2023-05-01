using DG.Tweening;
using SpookuleleAudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
    // components
    private CanvasGroup canvasGroup;
    public GameObject gameOverPanel;
    public ASoundContainer gameOverStinger;

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
        
        canvasGroup.alpha = 1;
        
        gameOverStinger.Play();
    }
}
