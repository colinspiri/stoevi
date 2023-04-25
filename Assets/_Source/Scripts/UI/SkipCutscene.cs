using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;

public class SkipCutscene : MonoBehaviour {
    // components
    private InputActions inputActions;
    public Slider slider;
    private DialogueRunner dialogueRunner;

    // constants
    public bool callEvent;
    public SceneReference nextScene;
    public UnityEvent nextEvent;
    public GameObject skipPrompt;
    public float skipTime;
    
    // state
    private bool showSkipPrompt;
    private float skipTimer;

    private void Awake() {
        inputActions = new InputActions();
    }

    private void OnEnable() {
        inputActions.Enable();
    }
    private void OnDisable() {
        inputActions.Disable();
    }

    private void Start() {
        showSkipPrompt = false;
        skipPrompt.SetActive(false);
        
        dialogueRunner = FindObjectOfType<DialogueRunner>();
    }

    private void Update() {
        if (inputActions.UI.RightClick.ReadValue<float>() > 0) {
            showSkipPrompt = true;
            skipTimer += Time.deltaTime;
            
            if (skipTimer >= skipTime) {
                Skip();
            }
        }
        else skipTimer = 0f;
        
        UpdateSlider();

        if (showSkipPrompt) skipPrompt.SetActive(true);
    }

    private void Skip() {
        dialogueRunner.onNodeComplete.Invoke(dialogueRunner.CurrentNodeName);
        dialogueRunner.Stop();

        /*if (callEvent) {
            nextEvent.Invoke();
        }
        else {
            SceneManager.LoadScene(nextScene);
        }*/
    }

    private void UpdateSlider() {
        slider.value = skipTimer / skipTime;
    }
}
