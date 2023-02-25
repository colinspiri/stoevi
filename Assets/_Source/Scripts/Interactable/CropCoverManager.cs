using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropCoverManager : MonoBehaviour {
    public Cover cover;
    public Rustle rustle;
    public List<GameObject> adjustHeight;
    
    // constants
    public float sproutHeight;
    public float halfHeight;
    public float fullHeight;

    public void UpdateCover(Crop.GrowthStage stage) {
        float newHeight = stage switch {
            Crop.GrowthStage.Sprout => sproutHeight,
            Crop.GrowthStage.Intermediate => halfHeight,
            Crop.GrowthStage.Unripe => fullHeight,
            Crop.GrowthStage.Ripe => fullHeight,
            Crop.GrowthStage.Bare => fullHeight,
            _ => 1f
        };

        transform.localScale = new Vector3(transform.localScale.x, newHeight, transform.localScale.z);
        foreach (var obj in adjustHeight) {
            obj.transform.localScale = new Vector3(obj.transform.localScale.x, newHeight, obj.transform.localScale.z);
        }

        // disable cover for sprouts
        if (stage == Crop.GrowthStage.Sprout) {
            cover.enabled = false;
            rustle.enabled = false;
        }
        else {
            cover.enabled = true;
            rustle.enabled = true;
        }
    }
}
