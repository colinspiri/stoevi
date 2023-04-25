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
    private bool conversationPlaying;
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
        if (nextConversation >= conversations.Count) return;
        if (conversationPlaying) return;

        if (TorbalanHearing.Instance != null && FirstPersonMovement.Instance != null) {
            distanceFromTorbalan = Vector3.Distance(FirstPersonMovement.Instance.transform.position, TorbalanHearing.Instance.transform.position);
            if (distanceFromTorbalan < distanceThreshold) return;
        }

        if (waitTimer <= 0) {
            StartNextConversation();
        }
        else {
            waitTimer -= Time.deltaTime;
        }
    }

    public bool TryStartConversation(string nodeName) {
        if (dialogueRunner.IsDialogueRunning) return false;

        if (distanceThreshold < distanceThreshold) return false;
        
        dialogueRunner.StartDialogue(nodeName);
        conversationPlaying = true;
        return true;
    }

    private void StartNextConversation() {
        // start conversation
        dialogueRunner.StartDialogue(conversations[nextConversation]);
        nextConversation++;
        conversationPlaying = true;
    }

    private void OnConversationDone() {
        conversationPlaying = false;
        // start wait timer
        waitTimer = Random.Range(minWaitTime, maxWaitTime);
    }
}
