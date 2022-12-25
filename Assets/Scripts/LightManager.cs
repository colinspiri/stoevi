using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[ExecuteAlways]
public class LightManager : MonoBehaviour {
    // components
    public static LightManager Instance;
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField] private Camera Camera;

    // state
    [SerializeField, Range(0, 1)] private float TimePercent;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        TryGetComponents();
    }

    // Update is called once per frame
    void Update() {
        if (Preset == null) return;

        // update lighting in editor
        if (!Application.isPlaying) {
            UpdateLighting(TimePercent);
        }
    }

    public void UpdateLighting(float timePercent) {
        TryGetComponents();
        
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);

        if (DirectionalLight != null) {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
        }
        
        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);
        RenderSettings.fogEndDistance = Mathf.Lerp(Preset.FogMaxDistanceDay, Preset.FogMaxDistanceNight, timePercent);

        if (Camera != null) {
            Camera.backgroundColor = Preset.FogColor.Evaluate(timePercent);
        }
    }

    private void OnValidate() {
        TryGetComponents();
    }

    private void TryGetComponents() {
        // get camera
        if (Camera == null) {
            Camera = FindObjectOfType<Camera>();
        }
        
        if (DirectionalLight != null) return;

        if (RenderSettings.sun != null) {
            DirectionalLight = RenderSettings.sun;
        }
        else {
            Light[] lights = GameObject.FindObjectsOfType<Light>();
            foreach (Light light in lights) {
                if (light.type == LightType.Directional) {
                    DirectionalLight = light;
                    return;
                }
            }
        }
    }
}
