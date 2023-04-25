using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {
    public string yarnNode;
    private bool triggered;

    private void OnTriggerStay(Collider other) {
        if (triggered) return;

        if (FirstPersonMovement.Instance == null || ConversationManager.Instance == null) return;

        if (other.gameObject == FirstPersonMovement.Instance.gameObject) {
            triggered = ConversationManager.Instance.TryStartConversation(yarnNode);
        }
    }
}
