using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTorbalanEyeColor : MonoBehaviour
{
    public Material eyesMaterial;
    public float intensity;

    // Start is called before the first frame update
    void Start()
    {
        Color emissiveColor = Color.red * Mathf.GammaToLinearSpace(intensity);
        eyesMaterial.SetColor("_EmissionColor", emissiveColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
