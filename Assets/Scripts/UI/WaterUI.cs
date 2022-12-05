using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WaterUI : MonoBehaviour
{
    // components
    public Slider waterSlider;
    private CanvasGroup canvasGroup;
    
    // constants
    public float fadeInTime;
    public float valueLerpTime;
    public float waitTime;
    public float fadeOutTime;

    // Start is called before the first frame update
    void Start() {
        canvasGroup = waterSlider.GetComponent<CanvasGroup>();
        waterSlider.value = 1;
        canvasGroup.alpha = 0;
        
        if(GameManager.Instance) GameManager.Instance.onWaterValueChange.AddListener(waterValue => {
            canvasGroup.DOFade(1, fadeInTime);
            Sequence sequence = DOTween.Sequence();
            sequence.Append(waterSlider.DOValue(waterValue, valueLerpTime));
            sequence.AppendInterval(waitTime);
            sequence.Append(canvasGroup.DOFade(0, fadeOutTime));
        });
    }
    
    
}
