using UnityEngine;

[RequireComponent(typeof(Light))]
public class TorbalanSpotLight : MonoBehaviour {
    // components
    private Light light;
    
    // shared state
    public FloatReference torbalanAwareness;
    
    // constants
    public float minBrightness; // 50
    public float maxBrightness; // 100

    private void Awake() {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        Color color;
        float brightness;
        if (torbalanAwareness.Value >= 1) {
            color = Color.red;
            brightness = maxBrightness;
        }
        else if (torbalanAwareness.Value >= 0.5f) {
            color = Color.red;
            float t = (torbalanAwareness.Value - 0.5f) * 2f;
            brightness = Mathf.Lerp(minBrightness, maxBrightness, t);
        }
        else if (torbalanAwareness.Value > 0) {
            float t = torbalanAwareness.Value * 2f;
            color = Color.Lerp(Color.yellow, Color.red, t);
            brightness = minBrightness;
        }
        else {
            color = Color.yellow;
            brightness = minBrightness;
        }
        light.color = color;
        light.intensity = brightness;
    }
}
