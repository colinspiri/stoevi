using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour {
    public List<Cutscene> cutscenes;
    
    // Start is called before the first frame update
    void Start() {
        foreach (Cutscene cutscene in cutscenes) {
            cutscene.gameObject.SetActive(false);
        }
        
        int currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        
        int index = currentDay - 1;
        // if don't have a cutscene, just play first cutscene
        if (index > cutscenes.Count) index = 0;

        cutscenes[index].gameObject.SetActive(true);
    }
}
