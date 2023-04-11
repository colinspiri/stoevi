using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TorbalanStateTracker : MonoBehaviour {
    public static TorbalanStateTracker Instance;
    
    public enum TorbalanState { Backstage, Idle, Frontstage, Search, Chase}
    public TorbalanState currentState;

    public UnityEvent<TorbalanState> onStateChange;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        onStateChange.AddListener(state => {
            Debug.Log("Torbalan state = " + state.ToString());
        });
    }

    public void SetBackstage() {
        SetState(TorbalanState.Backstage);
    }
    public void SetIdle() {
        SetState(TorbalanState.Idle);
    }
    public void SetFrontstage() {
        SetState(TorbalanState.Frontstage);
    }
    public void SetSearch() {
        SetState(TorbalanState.Search);
    }
    public void SetChase() {
        SetState(TorbalanState.Chase);
    }

    private void SetState(TorbalanState newState) {
        if (currentState == newState) return;

        currentState = newState;
        onStateChange.Invoke(currentState);
    }
}
