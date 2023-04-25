using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour {
    public bool playInEditor;
    public Cutscene cutscene;
    
    // Start is called before the first frame update
    void Start() {
        cutscene.gameObject.SetActive(false);

        if (!Application.isEditor || playInEditor) {
            int currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        
            cutscene.gameObject.SetActive(true);
        }
    }
}
