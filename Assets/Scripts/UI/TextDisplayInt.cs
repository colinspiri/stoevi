using TMPro;
using UnityEngine;

public class TextDisplayInt : MonoBehaviour
{
    // components
    public TextMeshProUGUI text;
    public IntReference intReference;
    
    // state
    private int previousValue = -1;

    private void OnEnable() {
        UpdateText();
    }

    private void Update() {
        if (previousValue != intReference.Value) {
            UpdateText();
            previousValue = intReference.Value;
        }
    }

    private void UpdateText() {
        text.text = intReference.Value.ToString();
    }
}