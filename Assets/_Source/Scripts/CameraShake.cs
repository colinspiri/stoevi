using System.Collections;
using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour {
    // components
    public static CameraShake Instance;
    private CinemachineVirtualCamera camera;
    private CinemachineBasicMultiChannelPerlin perlin;

    public float duration = 0.1f;
    public float intensity = 1f;

    private void Awake() {
        if (Instance == null) Instance = this;
        camera = GetComponent<CinemachineVirtualCamera>();
        perlin = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start() {
        perlin.m_AmplitudeGain = 0;
    }

    public void Shake() {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine(float multiplier = 1f) {
        perlin.m_AmplitudeGain = intensity * multiplier;

        float elapsed = 0.0f;
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            yield return null;
        }

        perlin.m_AmplitudeGain = 0;
    }
}