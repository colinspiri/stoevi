using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class LightManager : MonoBehaviour {
    // components
    private Camera Camera;
    private Light DirectionalLight;
    [SerializeField] private LightingPreset Preset;

    // time of day
    [SerializeField] private TimeOfDay timeOfDay;
    [SerializeField, Range(0, 1)] private float TimePercent;

    // lightning
    private bool flashing;
    private float lightningTimer;

    private void Start() {
        TryGetComponents();
        
        lightningTimer = Random.Range(Preset.minLightningTime, Preset.maxLightningTime);
    }

    // Update is called once per frame
    void Update() {
        if (Preset == null) return;

        if(Application.isPlaying) {
            if (!flashing) {
                UpdateLighting(timeOfDay ? timeOfDay.GetTimePercent() : TimePercent);
            }

            // lightning
            if (Preset.lightningEnabled) {
                lightningTimer -= Time.deltaTime;
                if (lightningTimer <= 0) {
                    StartCoroutine(FlashColor(Preset.lightningColor));
                    lightningTimer = Random.Range(Preset.minLightningTime, Preset.maxLightningTime);
                }
            }
        }
        // update lighting in editor
        else {
            UpdateLighting(TimePercent);
        }
    }

    private IEnumerator FlashColor(Color color) {
        flashing = true;

        if(DirectionalLight != null) DirectionalLight.color = color;
        RenderSettings.fogColor = color;
        if (Camera != null) {
            Camera.backgroundColor = color;
        }

        yield return new WaitForSeconds(Preset.lightningDuration);
        
        flashing = false;
    }

    private void UpdateLighting(float timePercent) {
        TryGetComponents();
        
        RenderSettings.ambientLight = Preset.AmbientColor.Evaluate(timePercent);

        if (DirectionalLight != null) {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
            DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Lerp(Preset.StartingLightDirection, Preset.EndingLightDirection, timePercent), 170f, 0));
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
            Camera = Camera.main;
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
