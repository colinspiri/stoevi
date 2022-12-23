using System.Collections;
using System.Collections.Generic;
using SpookuleleAudio;
using UnityEngine;

public class SheepManager : MonoBehaviour {
    // constants
    public static SheepManager Instance;
    public ASoundContainer bleat;
    
    // constants
    public float scareCooldownMin;
    public float scareCooldownMax;
    public float minScareDistance;

    // state
    private List<Sheep> allSheep = new List<Sheep>();
    private float scareTimer;
    
    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        ResetTimer();
    }

    // Update is called once per frame
    void Update() {
        scareTimer -= Time.deltaTime;
        
        if (scareTimer <= 0) {
            Scare();
        }
    }

    private void ResetTimer() {
        scareTimer = Random.Range(scareCooldownMin, scareCooldownMax);
    }

    private void Scare() {
        // find a sheep that's far away and not within sight
        Sheep chosenSheep = null;
        float maxDistance = 0;
        foreach (var sheep in allSheep) {
            // check if player looking
            Vector3 playerToSheep = sheep.transform.position - FirstPersonMovement.Instance.transform.position;
            var angle = Vector3.Angle(FirstPersonMovement.Instance.transform.forward, playerToSheep);
            if (angle < 90) continue;
            
            // has to be farther than min scare distance
            var distance = Vector3.Distance(FirstPersonMovement.Instance.transform.position, sheep.transform.position);
            if (distance < minScareDistance) continue;

            // look for farthest sheep
            if (distance > maxDistance) {
                maxDistance = distance;
                chosenSheep = sheep;
            }

            break;
        }

        if (chosenSheep == null) return;
        

        // teleport sheep behind player
        var pos = FirstPersonMovement.Instance.transform.position -
                  2 * FirstPersonMovement.Instance.transform.forward + 1 * Vector3.up;
        chosenSheep.transform.position = pos;
        
        // play stinger and bleat
        bleat.Play3D(transform);

        // reset timer
        ResetTimer();
    }

    public void AddSheep(Sheep sheep) {
        allSheep.Add(sheep);
    }
    public void RemoveSheep(Sheep sheep) {
        allSheep.Remove(sheep);
    }
}
