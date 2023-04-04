using UnityEngine;

public class ResourceManager : MonoBehaviour {
    // water
    public IntVariable maxWater;
    public IntVariable currentWater;
    
    // tomatoes
    public IntVariable playerTomatoes;
    public IntVariable torbalanTomatoes;
    
    // seeds
    public IntReference startingSeeds;
    public IntVariable seeds;

    // Start is called before the first frame update
    void Start() {
        currentWater.SetValue(maxWater.Value);
        
        playerTomatoes.SetValue(0);
        torbalanTomatoes.SetValue(0);

        int day = PlayerPrefs.GetInt("CurrentDay", 1);
        if(day == 1) seeds.SetValue(startingSeeds.Value);

    }
}
