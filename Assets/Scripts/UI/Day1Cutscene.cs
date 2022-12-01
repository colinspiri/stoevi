using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class Day1Cutscene : MonoBehaviour {
    public float fadeTime;

    [FormerlySerializedAs("squareBackground")] public CanvasGroup canvasGroup;
    public TextMeshProUGUI day1Text;

    // Start is called before the first frame update
    void Start() {
        canvasGroup.gameObject.SetActive(true);
        day1Text.alpha = 0;

        GameManager.Instance.Pause();

        StartCoroutine(Day1CutsceneCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator Day1CutsceneCoroutine() {
        AudioManager.Instance.PlayWalking();

        yield return new WaitForSecondsRealtime(2);
        
        day1Text.alpha = 1;
        // play sheep baa 
        AudioManager.Instance.PlayChaseSound();

        yield return new WaitForSecondsRealtime(2);
        
        // play gate sfx
        
        // fade into gameplay
        GameManager.Instance.Resume();
        Tweener backgroundTween = canvasGroup.DOFade(0, fadeTime).SetUpdate(true);
        yield return backgroundTween.WaitForCompletion();
        
        Destroy(gameObject);

        yield return null;
    }
}
