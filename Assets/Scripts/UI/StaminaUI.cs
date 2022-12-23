using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    // components
    private Slider slider;
    private CanvasGroup canvasGroup;
    public Image fill;
    
    // constants
    public Color recoveringColor;
    public float valueLerpTime;
    public float waitTime;
    public float fadeOutTime;
    
    // state
    private Color normalColor;
    private float waitTimer;
    private Tween fadeOutTween;

    private void Awake() {
        slider = GetComponent<Slider>();
        canvasGroup = GetComponent<CanvasGroup>();
        normalColor = fill.color;
    }

    // Start is called before the first frame update
    void Start() {
        slider.value = 1;
        canvasGroup.alpha = 0;

        StaminaController.OnStaminaChange += stamina => {
            ShowUI();
            
            slider.DOValue(stamina, valueLerpTime);
        };
        StaminaController.OnStateChange += state => {
            ShowUI();
            
            if (state == StaminaController.StaminaState.Recovering) {
                fill.color = recoveringColor;
            }
            else fill.color = normalColor;
        };
    }

    private void Update() {
        // if at maximum, fade out after some time
        if (slider.value >= 1 && waitTimer > 0) {
            waitTimer -= Time.deltaTime;
            
            if (waitTimer <= 0) {
                fadeOutTween = canvasGroup.DOFade(0, fadeOutTime);
                fadeOutTween.onComplete += () => fadeOutTween = null;
                fadeOutTween.onKill += () => fadeOutTween = null;
            }
        }
    }

    private void ShowUI() {
        if (canvasGroup.alpha < 1) {
            canvasGroup.alpha = 1;
            if (fadeOutTween != null) fadeOutTween.Kill();
        }
        waitTimer = waitTime;
    }
}
