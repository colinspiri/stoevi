using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Selectable))]
public class MenuOptionAnimator : MonoBehaviour {
    // components
    private Selectable selectable;
    public TextMeshProUGUI text;
    
    // constants
    public UIConstants uiConstants;

    private void Awake() {
        selectable = GetComponent<Selectable>();
    }

    public void Submit() {
        ExecuteEvents.Execute(selectable.gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }

    public void DeselectAnimation() {
        selectable.transform.DOScale(1f, uiConstants.selectTime).SetUpdate(true);
        text.DOColor(Color.white, uiConstants.selectTime);
    }

    public void SelectAnimation() {
        selectable.transform.DOScale(uiConstants.scaleOnSelect, uiConstants.selectTime).SetUpdate(true);
        text.DOColor(Color.red, uiConstants.selectTime);
        PlaySelectSound();
    }

    public void SubmitAnimation()
    {
        // to be implemented
    }

    private void PlaySelectSound()
    {
        // if (AudioManager.Instance) AudioManager.Instance.PlayUI(uiConstants.selectSFX);
    }
    
    public void PlaySubmitSound()
    {
        // if(AudioManager.Instance) AudioManager.Instance.PlayUI(uiConstants.submitSFX);
    }
    public void PlayBackSound()
    {
        // if(AudioManager.Instance) AudioManager.Instance.PlayUI(uiConstants.backSFX);
    }
}