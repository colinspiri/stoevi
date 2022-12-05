using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : Interactable
{

    public override void Interact() {
        Debug.Log("soil interact");
    }
    
    public override string GetUIText() {
        return "soil";
    }

    public override float GetSliderFloat() {
        return 0;
    }
}
