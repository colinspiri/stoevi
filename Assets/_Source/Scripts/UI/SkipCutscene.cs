using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SkipCutscene : MonoBehaviour {
    // constants
    public bool callEvent;
    public SceneReference nextScene;
    public UnityEvent nextEvent;
    public GameObject skipPrompt;
    
    // state
    public bool canSkip;

    private void Start() {
        skipPrompt.SetActive(false);
    }

    private void Update() {
        if (Keyboard.current.anyKey.wasPressedThisFrame) {
            if (canSkip) Skip();
            else {
                canSkip = true;
                skipPrompt.SetActive(true);
            }
        }
    }

    private void Skip() {
        if (callEvent) {
            nextEvent.Invoke();
        }
        else {
            SceneManager.LoadScene(nextScene);
        }
    }
}
