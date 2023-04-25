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

        if (cutscenesByDay.ContainsKey(currentDay)) {
            string cutsceneNode = cutscenesByDay[currentDay];
            
            // get dialogue runner
            dialogueRunner = FindObjectOfType<DialogueRunner>();
            if (dialogueRunner != null) {
                dialogueRunner.onNodeComplete.AddListener(_ => OnCutsceneDone());
                dialogueRunner.StartDialogue(cutsceneNode);
            }
            else OnCutsceneDone();
        }
        else {
            OnCutsceneDone();
        }

        Debug.Log("playing cutscene for day = " + currentDay);
    }
    
    private void OnCutsceneDone() {
        int currentDay = PlayerPrefs.GetInt("CurrentDay", 1);

        if (currentDay >= 6) {
            sceneLoader.LoadEndCredits();
        }
        else sceneLoader.LoadCurrentDay();
    }
}
