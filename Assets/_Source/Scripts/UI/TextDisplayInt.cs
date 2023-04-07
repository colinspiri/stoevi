using DG.Tweening;
using SpookuleleAudio;
using TMPro;
using UnityEngine;

public class TextDisplayInt : MonoBehaviour
{
    // components
    public TextMeshProUGUI text;
    public IntReference intReference;
    
    // constants
    public int modifier = 0;
    public bool animateOnUpdate;
    
    // state
    private int previousValue = -1;

    private void Start() {
        UpdateText(false);
    }

    private void Update() {
        if (previousValue != intReference.Value) {
            UpdateText(animateOnUpdate);
        }
    }

    private void UpdateText(bool animate) {
        text.text = (intReference.Value + modifier).ToString();
        previousValue = intReference.Value;

        if (animate) {
            text.transform.DOShakeScale(1).OnComplete((() => text.transform.localScale = Vector3.one));
        }
    }
}