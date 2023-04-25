using TMPro;
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
    public TextMeshProUGUI skipPrompt;
    public float skipPromptAlpha;
    public float skipTime;
    [Header("On Skip")]
    public bool useEvent;
    public UnityEvent nextEvent;
    public bool useScene;
    public SceneReference nextScene;
    public bool useYarnCallback;
    
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
        showSkipPrompt = true;
        skipPrompt.gameObject.SetActive(false);
        
        dialogueRunner = FindObjectOfType<DialogueRunner>();
    }

    private void Update() {
        if (showSkipPrompt) {
            skipPrompt.gameObject.SetActive(true);
            skipPrompt.alpha = skipPromptAlpha;
        }
        
        if (inputActions.UI.RightClick.ReadValue<float>() > 0) {
            showSkipPrompt = true;
            skipPrompt.alpha = 1;
            skipTimer += Time.deltaTime;
            
            if (skipTimer >= skipTime) {
                Skip();
            }
        }
        else skipTimer = 0f;
        
        UpdateSlider();
    }

    private void Skip() {
        if (useEvent) {
            nextEvent.Invoke();
        }
        else if(useScene) {
            SceneManager.LoadScene(nextScene);
        }
        else if (useYarnCallback) {
            dialogueRunner.onNodeComplete.Invoke(dialogueRunner.CurrentNodeName);
            dialogueRunner.Stop();
        }
    }

    private void UpdateSlider() {
        slider.value = skipTimer / skipTime;
    }
}
