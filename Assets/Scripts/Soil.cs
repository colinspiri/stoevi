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
        // check if player has seeds left
        if (!ResourceManager.Instance.HasSeedsLeft()) return;
        
        // check if space left
        if (crops.Count >= maxCrops) return;
        
        // get player look position
        var position = CameraRaycast.Instance.GetCurrentHitPosition();
        
        // spawn crop
        Crop newCrop = Instantiate(seedPrefab, position, transform.rotation).GetComponent<Crop>();
        crops.Add(newCrop);

        // use seed
        ResourceManager.Instance.UseSeed();
    }

    public override string GetUIText() {
        if (crops.Count >= maxCrops) return "no more space";
        if (!ResourceManager.Instance.HasSeedsLeft()) return "out of seeds";
        return "E to plant seed";
    }
}
