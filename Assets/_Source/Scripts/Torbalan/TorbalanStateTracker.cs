using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorbalanStateTracker : MonoBehaviour
{
    public enum TorbalanState { Backstage, Idle, Frontstage, Search, Chase}
    public TorbalanState currentState;

    public void SetBackstage() {
        currentState = TorbalanState.Backstage;
    }

    public void SetIdle() {
        currentState = TorbalanState.Idle;
    }

    public void SetFrontstage() {
        currentState = TorbalanState.Frontstage;
    }

    public void SetSearch() {
        currentState = TorbalanState.Search;
    }

    public void SetChase() {
        currentState = TorbalanState.Chase;
    }
}
