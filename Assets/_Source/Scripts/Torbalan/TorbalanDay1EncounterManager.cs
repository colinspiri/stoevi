using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class TorbalanDay1EncounterManager : MonoBehaviour {
    private DialogueRunner dialogueRunner;
    
    public bool Encounter { get; set; }
    
    public List<float> encounterTimesInMinutes;

    private int encounterIndex;
    private float timer;

    public List<string> encounterDialogues;

    private void Start() {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
    }

    // Update is called once per frame
    void Update() {
        if (encounterIndex >= encounterTimesInMinutes.Count) return;

        if (timer >= encounterTimesInMinutes[encounterIndex] * 60f) {
            Encounter = true;
        }
        else {
            timer += Time.deltaTime;
        }
    }

    public void EndEncounter() {
        // play dialogue
        if (dialogueRunner == null) {
            dialogueRunner = FindObjectOfType<DialogueRunner>();
        }
        if (dialogueRunner != null) {
            string yarnNode = encounterIndex < encounterDialogues.Count ? encounterDialogues[encounterIndex] : "";
            if (yarnNode != "") {
                dialogueRunner.Stop();
                dialogueRunner.StartDialogue(yarnNode);
            }
        }
        
        
        Encounter = false;
        encounterIndex++;
    }
}
