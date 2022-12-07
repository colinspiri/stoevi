using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soil : Interactable {
    // components
    public GameObject seedPrefab;
    
    // constants
    public int maxCrops;
    
    // state
    private List<Crop> crops = new List<Crop>();

    public override void Interact() {
        // get player look position
        var position = CameraRaycast.Instance.GetCurrentHitPosition();
        
        // spawn crop
        Crop newCrop = Instantiate(seedPrefab, position, transform.rotation).GetComponent<Crop>();
        crops.Add(newCrop);

        if (crops.Count >= maxCrops) {
            SetInteractable(false);
        }
    }

    public override string GetUIText() {
        return "E to plant seed";
    }
}
