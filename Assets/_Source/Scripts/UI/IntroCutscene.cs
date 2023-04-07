using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SpookuleleAudio;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class IntroCutscene : MonoBehaviour {
    public float fadeTime;
    [FormerlySerializedAs("waitTime")] public float lineWaitTime;

    [Header("Title")]
    public CanvasGroup titlePanel;
    public GameObject titleText;

    [Header("Credits")]
    public List<CanvasGroup> creditsPanels;

    [Header("Setting")]
    public GameObject settingPanel;
    public TextMeshProUGUI bulgariaText;
    public TextMeshProUGUI levnikText;
    public TextMeshProUGUI everyoneText;
    public TextMeshProUGUI exceptText;

    [Header("Scenes")]
    public SceneReference day1Scene;
    
    [Header("Audio")]
    public AudioSource ambience_day;
    public float fadeInTime;
    public ASoundContainer ui_play;

    // Start is called before the first frame update
    void Start()
    {
        titlePanel.gameObject.SetActive(true);
        titleText.SetActive(false);

        /*credits1Panel.gameObject.SetActive(false);
        credits1Panel.alpha = 0;*/
        foreach (var creditsPanel in creditsPanels) {
            creditsPanel.gameObject.SetActive(false);
            creditsPanel.alpha = 0;
        }

        settingPanel.SetActive(false);
        bulgariaText.alpha = 0;
        levnikText.alpha = 0;
        everyoneText.alpha = 0;
        exceptText.alpha = 0;

        StartCoroutine(IntroCutsceneCoroutine());
    }

    private IEnumerator IntroCutsceneCoroutine() {
        yield return new WaitForSeconds(2);

        // title
        titleText.SetActive(true);
        ui_play.Play();
        AudioManager.Instance.PlayIntroCutsceneMusic();

        yield return new WaitForSeconds(2);
        
        Tween titlePanelTween = titlePanel.DOFade(0, fadeTime);
        yield return titlePanelTween.WaitForCompletion();
        titlePanel.gameObject.SetActive(false);
        
        // credits
        foreach (var creditsPanel in creditsPanels) {
            creditsPanel.gameObject.SetActive(true);
            creditsPanel.DOFade(1, fadeTime);

            yield return new WaitForSeconds(3);

            Tween _creditsPanelTween = creditsPanel.DOFade(0, fadeTime);
            yield return _creditsPanelTween.WaitForCompletion();
            creditsPanel.gameObject.SetActive(false);
        }
        /*credits1Panel.gameObject.SetActive(true);

        credits1Panel.DOFade(1, fadeTime);

        yield return new WaitForSeconds(3);

        Tween creditsPanelTween = credits1Panel.DOFade(0, fadeTime);
        yield return creditsPanelTween.WaitForCompletion();
        credits1Panel.gameObject.SetActive(false);*/
        
        // setting
        settingPanel.SetActive(true);
        AudioManager.Instance.PlayDayAmbience(20);

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

        SceneManager.LoadScene(day1Scene.ScenePath);
        
        yield return null;
    }
}
