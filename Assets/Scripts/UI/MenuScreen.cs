using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuScreen : MonoBehaviour {
    // components
    public RectTransform rectTransform;
    public EventSystem eventSystem;
    public CanvasGroup canvasGroup;

    // constants
    public UIConstants uiConstants;

    public void StartOffscreen() {
        rectTransform.anchoredPosition = new Vector2(uiConstants.offscreenDistance, 0);
        canvasGroup.alpha = 0;
        DisableFunctionality();
    }

    public void StartOnscreen() {
        rectTransform.anchoredPosition = new Vector2(0, 0);
        canvasGroup.alpha = 1;
        EnableFunctionality();
    }

    public void Push() {
        rectTransform.DOAnchorPosX(0, uiConstants.menuScreenTransitionTime).SetUpdate(true).onComplete +=
            EnableFunctionality;
        canvasGroup.DOFade(1, uiConstants.menuScreenTransitionTime).SetUpdate(true).SetEase(Ease.OutSine);
    }

    public void PutAway() {
        DisableFunctionality();
        rectTransform.DOAnchorPosX(-uiConstants.offscreenDistance, uiConstants.menuScreenTransitionTime)
            .SetUpdate(true);
        canvasGroup.DOFade(0, uiConstants.menuScreenTransitionTime).SetUpdate(true).SetEase(Ease.OutSine);
    }

    public void Pop() {
        DisableFunctionality();
        rectTransform.DOAnchorPosX(uiConstants.offscreenDistance, uiConstants.menuScreenTransitionTime).SetUpdate(true);
        canvasGroup.DOFade(0, uiConstants.menuScreenTransitionTime).SetUpdate(true).SetEase(Ease.OutSine);
    }

    public void BringBack() {
        rectTransform.DOAnchorPosX(0, uiConstants.menuScreenTransitionTime).SetUpdate(true).onComplete +=
            EnableFunctionality;
        canvasGroup.DOFade(1, uiConstants.menuScreenTransitionTime).SetUpdate(true).SetEase(Ease.OutSine);
    }

    private void EnableFunctionality() {
        eventSystem.gameObject.SetActive(true);

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void DisableFunctionality() {
        eventSystem.gameObject.SetActive(false);

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}