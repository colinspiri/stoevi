using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorbalanDirector : MonoBehaviour {
    // constants
    public float farRadius;
    public float closeRadius;
    [Tooltip("Heat value when Torbalan backs off")]
    public float heatThreshold;
    public float maxTimeAwayFromPlayer;
    
    // state
    private bool directorCommandGiven;
    private float heat;
    private float timeAwayFromPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (heat >= heatThreshold) {
            directorCommandGiven = true;
            Debug.Log("Director command given: back off!");
        }
        if (timeAwayFromPlayer >= maxTimeAwayFromPlayer) {
            directorCommandGiven = true;
            Debug.Log("Director command given: go to player!");
        }
        if (!directorCommandGiven) {
            var distance = Vector3.Distance(FirstPersonController.Instance.transform.position, transform.position);
            if (distance > farRadius) {
                timeAwayFromPlayer += Time.deltaTime;
            }
            else if (distance < closeRadius) {
                heat += Time.deltaTime;
            }
        }
    }
}
