using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CutsceneStart : MonoBehaviour {
    // variables
    public SerializedDictionary<int, string> cutscenesByDay;
    
    // components
    public SceneLoader sceneLoader;
    private DialogueRunner dialogueRunner;

    private void Start() {
        int currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        int lookupDay = currentDay - 1;

        if (cutscenesByDay.ContainsKey(lookupDay)) {
            string cutsceneNode = cutscenesByDay[lookupDay];
            
            // get dialogue runner
            dialogueRunner = FindObjectOfType<DialogueRunner>();
            if (dialogueRunner != null) {
                dialogueRunner.onNodeComplete.AddListener(_ => OnCutsceneDone());
            }
            dialogueRunner.StartDialogue(cutsceneNode);
        }

        Debug.Log("currentDay = " + currentDay);
    }
    
    private void OnCutsceneDone() {
        sceneLoader.LoadCurrentDay();
    }
}
