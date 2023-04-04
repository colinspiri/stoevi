using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class Cutscene : MonoBehaviour {
    // components
    private CanvasGroup canvasGroup;
    public TextMeshProUGUI dayText;
    
    // constants
    private static float fadeTime = 3f;

    private void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable() {
        StartCoroutine(EntireCutsceneCoroutine());
    }

    private IEnumerator EntireCutsceneCoroutine() {
        if(GameManager.Instance) GameManager.Instance.Pause();

        
        yield return StartCoroutine(CutsceneCoroutine());
        
        // fade into gameplay
        if(GameManager.Instance) GameManager.Instance.Resume();
        Tweener canvasGroupTween = canvasGroup.DOFade(0, fadeTime).SetUpdate(true);
        yield return canvasGroupTween.WaitForCompletion();
        
        Destroy(gameObject);

        yield return null;
    }

    protected abstract IEnumerator CutsceneCoroutine();
}