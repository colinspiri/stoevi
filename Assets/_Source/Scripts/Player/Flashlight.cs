using System;
using System.Collections;
using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour
{
    // components
    private InputActions inputActions;
    private Light light;
    public GameObject followObject;
    public ASoundContainer sfx_flashlight;
    
    // constants
    public float lerpSpeed;

    private void Awake() {
        light = GetComponent<Light>();
        
        inputActions = new InputActions();
        inputActions.Enable();
        inputActions.Gameplay.Flashlight.performed += context => {
            light.enabled = !light.enabled;
        };
    }

    // Start is called before the first frame update
    void Start() {
        light.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        transform.position = followObject.transform.position;
        transform.rotation =
            Quaternion.Lerp(transform.rotation, followObject.transform.rotation, lerpSpeed * Time.deltaTime);
    }
}
