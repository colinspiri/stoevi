using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour {
    public List<MeshRenderer> meshRenderers;

    public Material day1Material;
    public Material day3Material;
    public Material day4Material;
    public Material day5Material;
    
    // Start is called before the first frame update
    void Start() {
        UpdateRenderers();
    }

    private void UpdateRenderers() {
        int day = PlayerPrefs.GetInt("CurrentDay", 1);
        Material currentMaterial = day switch {
            5 => day5Material,
            4 => day4Material,
            3 => day3Material,
            2 => day1Material,
            1 => day1Material,
        };
        foreach (var meshRenderer in meshRenderers) {
            meshRenderer.material = currentMaterial;
        }
    }
}
