using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

public class IntroCutscene : MonoBehaviour {
    public float fadeTime;
    [FormerlySerializedAs("waitTime")] public float lineWaitTime;

    public CanvasGroup titlePanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI byText;
    
    public GameObject settingPanel;
    public TextMeshProUGUI bulgariaText;
    public TextMeshProUGUI levnikText;
    public TextMeshProUGUI everyoneText;
    public TextMeshProUGUI exceptText;

    // Start is called before the first frame update
    void Start()
    {
        titlePanel.gameObject.SetActive(true);
        titleText.alpha = 0;
        byText.alpha = 0;
        
        settingPanel.SetActive(false);
        bulgariaText.alpha = 0;
        levnikText.alpha = 0;
        everyoneText.alpha = 0;
        exceptText.alpha = 0;

        StartCoroutine(IntroCutsceneCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator IntroCutsceneCoroutine() {
        yield return new WaitForSeconds(2);

        titleText.alpha = 1;
        AudioManager.Instance.PlayChaseSound();

        yield return new WaitForSeconds(2);

        byText.alpha = 1;
        yield return new WaitForSeconds(1);

        Tween titlePanelTween = titlePanel.DOFade(0, fadeTime);
        yield return titlePanelTween.WaitForCompletion();
        
        titlePanel.gameObject.SetActive(false);
        
        // setting
        settingPanel.SetActive(true);
        AudioManager.Instance.PlayFarmAmbience();
        // fade in ambient nature SFX

        Tween bulgariaTween = bulgariaText.DOFade(1, fadeTime);
        yield return bulgariaTween.WaitForCompletion();
        yield return new WaitForSeconds(lineWaitTime);

        Tween levnikTween = levnikText.DOFade(1, fadeTime);
        yield return levnikTween.WaitForCompletion();
        yield return new WaitForSeconds(lineWaitTime);

        Tween everyoneTween = everyoneText.DOFade(1, fadeTime);
        yield return everyoneTween.WaitForCompletion();
        yield return new WaitForSeconds(lineWaitTime);

        Tween exceptTween = exceptText.DOFade(1, fadeTime);
        yield return exceptTween.WaitForCompletion();
        
        Debug.Log("done");
        
        yield return null;
    }
}
