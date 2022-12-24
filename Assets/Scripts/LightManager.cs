using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[ExecuteAlways]
public class LightManager : MonoBehaviour {
    // references
    [SerializeField] private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;
    [SerializeField] private Camera Camera;

    // state
    [SerializeField, Range(0, 24)] private float TimeOfDay;

    private void Start() {
        TryGetComponents();
    }

    // Update is called once per frame
    void Update() {
        if (Preset == null) return;

        if (Application.isPlaying) {
            if (TimeOfDay >= 24) TimeOfDay = 24;
            else TimeOfDay += Time.deltaTime;
        }
        UpdateLighting(TimeOfDay / 24f);
    }
    

    private void UpdateLighting(float timePercent) {
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
