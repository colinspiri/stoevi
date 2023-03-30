using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilMapIcon : MonoBehaviour {
    public Soil soil;
    public GameObject soilIcon;

    // Update is called once per frame
    void Update() {
        bool showIcon = soil.crops.Count == 0;
        soilIcon.SetActive(showIcon);
        
        // set rotation
        transform.rotation = Quaternion.Euler(-90, 180, 0);
    }
}
