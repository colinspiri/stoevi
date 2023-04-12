using DG.Tweening;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class FadePulse : MonoBehaviour {
    // components
    private TextMeshProUGUI text;
    
    // constants
    public float fadeTime;
    private void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start() {
        text.alpha = 0;
        StartFadingIn();
    }
    
    private void StartFadingIn() {
        text.DOFade(1, fadeTime).SetUpdate(true).OnComplete(StartFadingOut);
    }

    private void StartFadingOut() {
        text.DOFade(0, fadeTime).SetUpdate(true).OnComplete(StartFadingIn);
    }
}
