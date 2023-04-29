using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

public class PlayShopCutscene : MonoBehaviour
{
    // variables
    public SerializedDictionary<int, string> cutscenesByDay;
    public UnityEvent onDialogueComplete;
    
    // components
    private DialogueRunner dialogueRunner;
    
    private void OnEnable() {
        int day = PlayerPrefs.GetInt("CurrentDay", 1);
        Debug.Log("SHOP day = " + day);

        if (cutscenesByDay.ContainsKey(day)) {
            string cutsceneNode = cutscenesByDay[day];
            
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
    }
    
    private void OnCutsceneDone() {
        onDialogueComplete.Invoke();
    }
}
