using System;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using Random = UnityEngine.Random;

public class ConversationManager : MonoBehaviour {
    // public constants
    public List<string> conversations;
    public float distanceThreshold;
    public float minWaitTime;
    public float maxWaitTime;
    
    // components
    public static ConversationManager Instance;
    private DialogueRunner dialogueRunner;
    
    // state
    private int nextConversation = 0;
    private float distanceFromTorbalan;
    private float waitTimer;

    private void Awake() {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        dialogueRunner = FindObjectOfType<DialogueRunner>();
        if (dialogueRunner != null) {
            dialogueRunner.onNodeComplete.AddListener(_ => OnConversationDone());
        }

        waitTimer = Random.Range(minWaitTime, maxWaitTime);
    }

    // Update is called once per frame
    void Update() {
        if (dialogueRunner == null) {
            dialogueRunner = FindObjectOfType<DialogueRunner>();
            return;
        }
        if (dialogueRunner.IsDialogueRunning) return;

        // count wait timer
        if (waitTimer > 0) {
            waitTimer -= Time.deltaTime;
        }

        // check if backstage
        if (TorbalanDirector.Instance != null && !TorbalanDirector.Instance.Backstage) return;
        
        // check torbalan distance
        if (FirstPersonMovement.Instance == null || TorbalanDirector.Instance == null)
            distanceFromTorbalan = float.MaxValue;
        else distanceFromTorbalan = Vector3.Distance(FirstPersonMovement.Instance.transform.position, TorbalanHearing.Instance.transform.position);
        if (distanceFromTorbalan <= distanceThreshold) return;

        // try to start next conversation
        if (nextConversation < conversations.Count && waitTimer <= 0) {
            StartNextConversation();
        }
    }

    public bool TryStartConversation(string nodeName) {
        if (dialogueRunner.IsDialogueRunning) return false;
        if (distanceFromTorbalan < distanceThreshold) return false;
        
        dialogueRunner.StartDialogue(nodeName);
        return true;
    }

    private void StartNextConversation() {
        dialogueRunner.StartDialogue(conversations[nextConversation]);
        nextConversation++;
    }

    private void OnConversationDone() {
        waitTimer = Random.Range(minWaitTime, maxWaitTime);
    }
}