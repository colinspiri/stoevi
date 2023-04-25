using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SpookuleleAudio;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class EndCredits : MonoBehaviour {
    public float fadeTime;

    [Header("Title")]
    public CanvasGroup firstPanel;

    [Header("Credits")]
    public List<CanvasGroup> otherPanels;

    [Header("Scenes")]
    public SceneLoader sceneLoader;
    
    [Header("Audio")]
    public ASoundContainer ui_play;

    // Start is called before the first frame update
    void Start()
    {
        firstPanel.gameObject.SetActive(false);

        foreach (var creditsPanel in otherPanels) {
            creditsPanel.gameObject.SetActive(false);
            creditsPanel.alpha = 0;
        }

        StartCoroutine(IntroCutsceneCoroutine());
    }

    private IEnumerator IntroCutsceneCoroutine() {
        yield return new WaitForSeconds(2);

        // title
        firstPanel.gameObject.SetActive(true);
        firstPanel.alpha = 1;
        ui_play.Play();
        AudioManager.Instance.PlayThemeBassMusic();

        yield return new WaitForSeconds(2);
        
        Tween titlePanelTween = firstPanel.DOFade(0, fadeTime);
        yield return titlePanelTween.WaitForCompletion();
        firstPanel.gameObject.SetActive(false);
        
        // credits
        for (var i = 0; i < otherPanels.Count; i++) {
            var panel = otherPanels[i];
            panel.gameObject.SetActive(true);
            panel.DOFade(1, fadeTime);

            if (i == otherPanels.Count - 1) break;

            yield return new WaitForSeconds(3);

            Tween panelTween = panel.DOFade(0, fadeTime);
            yield return panelTween.WaitForCompletion();
            panel.gameObject.SetActive(false);
        }

        yield return null;
    }
}
