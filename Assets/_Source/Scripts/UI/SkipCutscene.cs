using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkipCutscene : MonoBehaviour {
    // components
    private InputActions inputActions;
    public Slider slider;

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
        if (callEvent) {
            nextEvent.Invoke();
        }
        else {
            SceneManager.LoadScene(nextScene);
        }
    }

    private void UpdateSlider() {
        slider.value = skipTimer / skipTime;
    }
}
