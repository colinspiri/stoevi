using TMPro;
using UnityEngine;

public class SummaryLineUI : MonoBehaviour
{
    // components
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI valueText;
    
    // state
    [Space] 
    public string description;
    public int value;

    private void OnEnable() {
        UpdateUI();
    }

    public void UpdateUI() {
        descriptionText.text = description;
        valueText.text = value.ToString();
    }
}
