using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TomatoNotification : MonoBehaviour {
    public static TomatoNotification Instance;
    
    private CanvasGroup canvasGroup;
    public TextMeshProUGUI text;
    
    public int tomatoes;

    private void Awake() {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    void Start() {
        canvasGroup.alpha = 0;
    }

    public void StartTomatoNotification() {
        text.text = "+" + tomatoes;

        canvasGroup.alpha = 1;

        canvasGroup.DOFade(0, 0.5f).SetDelay(2f);
    }
}
