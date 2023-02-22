using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TomatoNotification : MonoBehaviour {
    private CanvasGroup canvasGroup;
    public RectTransform rectTransform;
    public TextMeshProUGUI text;
    
    public IntReference playerTomatoes;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    void Start() {
        canvasGroup.alpha = 0;
    }

    public void StartTomatoNotification() {
        text.text = "+" + playerTomatoes.Value;

        canvasGroup.alpha = 1;

        canvasGroup.DOFade(0, 0.5f).SetDelay(2f);
    }
}
