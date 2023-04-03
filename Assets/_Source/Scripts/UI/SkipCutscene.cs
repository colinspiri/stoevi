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
        string cutsceneName = SceneManager.GetActiveScene().name;
        int playedAlready = PlayerPrefs.GetInt(cutsceneName, 0);
        if (playedAlready == 0) {
            canSkip = false;
            PlayerPrefs.SetInt(cutsceneName, 1);
        }
        else if(playedAlready == 1) {
            canSkip = true;
        }
        
        // enable/disable UI prompt
        skipPrompt.SetActive(canSkip);
    }

    private void Update() {
        if (canSkip && Keyboard.current.anyKey.wasPressedThisFrame) {
            Skip();
        }
    }

    private void Skip() {
        AudioManager.Instance.StopIntroCutsceneMusic();

        if (callEvent) {
            nextEvent.Invoke();
        }
        else {
            SceneManager.LoadScene(nextScene);
        }
    }
}
