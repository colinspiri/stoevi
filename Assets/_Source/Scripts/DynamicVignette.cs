using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DynamicVignette : MonoBehaviour
{
    // components
    public PostProcessProfile profile;
    public CoverSet coverSet;
    
    // constants
    public float defaultIntensity;
    public float hiddenIntensity;
    public float interpolationTime;

    // state
    private bool previousValue;

    // Update is called once per frame
    void Update()
    {
        if (previousValue != coverSet.PlayerInCompleteCover()) {
            UpdateVignette();
        }
    }

    private void UpdateVignette() {
        var vignette = profile.GetSetting<Vignette>();
        float startingIntensity = vignette.intensity.value;

        if (coverSet.PlayerInCompleteCover()) {
            DOTween.To(() => vignette.intensity.value, value => vignette.intensity.value = value, hiddenIntensity,
                interpolationTime);
            // vignette.intensity.Interp(startingIntensity, hiddenIntensity, interpolationTime);
        }
        else {
            DOTween.To(() => vignette.intensity.value, value => vignette.intensity.value = value, defaultIntensity,
                interpolationTime);
            // vignette.intensity.Interp(startingIntensity, defaultIntensity, interpolationTime);
        }
        
        previousValue = coverSet.PlayerInCompleteCover();
    }
}
