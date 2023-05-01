using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Yarn.Unity;

public class DialogueSpeakerPlacement : MonoBehaviour {
    // components
    public RectTransform rectTransform;
    public TextMeshProUGUI speakerText;
    
    // constants
    public float offset;
    public SerializedDictionary<string, int> orderOfSpeakers;
    
    // state
    private string currentSpeaker;
    private float startPosition;

    private void Start() {
        startPosition = rectTransform.anchoredPosition.y;
        currentSpeaker = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (speakerText.text != currentSpeaker) {
            UpdatePlacement();
            currentSpeaker = speakerText.text;
        }
    }

    private void UpdatePlacement() {
        if (speakerText.text == "") return;
        foreach (var pair in orderOfSpeakers) {
            if (speakerText.text.Contains(pair.Key)) {
                rectTransform.anchoredPosition = new Vector2(0, startPosition + offset * pair.Value);
            }
        }
    }
}
