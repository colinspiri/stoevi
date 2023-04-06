using DG.Tweening;
using SpookuleleAudio;
using TMPro;
using UnityEngine;

public class TextDisplayFloatPercent : MonoBehaviour
{
    // components
    public TextMeshProUGUI text;
    public FloatReference floatReference;
    
    // constants
    public bool animateOnUpdate;
    
    // state
    private float previousValue = -1;

    private void Start() {
        UpdateText(false);
    }

    private void Update() {
        if (previousValue != floatReference.Value) {
            UpdateText(animateOnUpdate);
        }
    }

    private void UpdateText(bool animate) {
        text.text = Mathf.Ceil(floatReference.Value * 100).ToString() + "%";
        previousValue = floatReference.Value;

        if (animate) {
            text.transform.DOShakeScale(1).OnComplete((() => text.transform.localScale = Vector3.one));
        }
    }
}