using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptionAnimator : MonoBehaviour {
    private Button button;
    public TextMeshProUGUI text;

    private void Awake() {
        button = GetComponent<Button>();
    }

    public void DeselectAnimation() {
        text.color = Color.white;
    }

    public void SelectAnimation() {
        text.color = Color.red;
    }

    public void Select() {
        button.Select();
    }
}
