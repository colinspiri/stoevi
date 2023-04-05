using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayCurrentDay : MonoBehaviour
{
    // components
    public TextMeshProUGUI text;

    private void OnEnable() {
        text.text = "Day " + PlayerPrefs.GetInt("CurrentDay", 1);
    }
}
