using UnityEngine;

public class CropCoverManager : MonoBehaviour {
    public Cover cover;
    public Rustle rustle;

    public void UpdateCover(Crop.GrowthStage stage) {
        // disable cover
        if (stage == Crop.GrowthStage.Sprout || stage == Crop.GrowthStage.Intermediate) {
            cover.enabled = false;
            rustle.enabled = false;
        }
        else {
            cover.enabled = true;
            rustle.enabled = true;
        }
    }
}
