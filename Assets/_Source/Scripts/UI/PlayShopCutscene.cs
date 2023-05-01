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
    
    // state
    private bool played;
    private float backupTimer;

    private void Update() {
        if (!played) {
            TryPlayCutscene();
            backupTimer += Time.deltaTime;

            if (backupTimer >= 2f) {
                OnCutsceneDone();
                played = true;
            }
        }
    }

    private void TryPlayCutscene() {
        int day = PlayerPrefs.GetInt("CurrentDay", 1);

        if (cutscenesByDay.ContainsKey(day)) {
            string cutsceneNode = cutscenesByDay[day];
            
            // get dialogue runner
            dialogueRunner = FindObjectOfType<DialogueRunner>();
            if (dialogueRunner != null && dialogueRunner.NodeExists(cutsceneNode)) {
                dialogueRunner.StartDialogue(cutsceneNode);
                dialogueRunner.onNodeComplete.AddListener(_ => OnCutsceneDone());
                played = true;
            }
        }
    }

    private void OnCutsceneDone() {
        onDialogueComplete.Invoke();
    }
}
