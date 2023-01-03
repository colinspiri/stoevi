using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AwarenessUI : MonoBehaviour {
    private Slider slider;
    public TorbalanSenses senses;

    private void Awake() {
        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update() {
        slider.value = senses.Awareness;
    }
}
