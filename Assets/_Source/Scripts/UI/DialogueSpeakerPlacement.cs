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
    private float startPosition;

    private void Start() {
        startPosition = rectTransform.anchoredPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var pair in orderOfSpeakers) {
            if (speakerText.text.Equals(pair.Key)) {
                rectTransform.anchoredPosition = new Vector2(0, startPosition + offset * pair.Value);
            }
        }
    }
}
