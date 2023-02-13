using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextDisplayInt : MonoBehaviour
{
    // components
    public TextMeshProUGUI text;
    public IntReference intReference;
    
    // constants
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
        text.text = intReference.Value.ToString();
        previousValue = intReference.Value;

        if (animate) {
            text.transform.DOShakeScale(1).OnComplete((() => text.transform.localScale = Vector3.one));
        }
    }
}