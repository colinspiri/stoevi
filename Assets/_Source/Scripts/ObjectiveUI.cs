using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectiveUI : MonoBehaviour {
    public static ObjectiveUI Instance;
    private InputActions inputActions;

    // public variables
    public TextMeshProUGUI text;
    public float timeToFadeAway;
    public IntReference objectiveComplete;
    public List<Prompt> allPrompts = new List<Prompt>();

    // private state
    private Prompt currentPrompt;
    private List<Prompt> queuedPrompts = new List<Prompt>();
    private bool promptActive;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("More than one instance of Instructions");
            return;
        }
        Instance = this;

        inputActions = new InputActions();
        inputActions.Enable();
    }

    // Start is called before the first frame update
    void Start() {
        // set text blank
        text.text = "";
        promptActive = false;

        // queue starting prompts
        int day = PlayerPrefs.GetInt("CurrentDay", 1);
        for (var i = 0; i < allPrompts.Count; i++)
        {
            var prompt = allPrompts[i];
            if (prompt.queueImmediately == false || prompt.day != day) break;

            queuedPrompts.Add(prompt);
            allPrompts.RemoveAt(i);
            i--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (promptActive == false) {
            if (queuedPrompts.Count > 0) {
                ShowNextPrompt();
            }
            else {
                if(ObjectiveManager.Instance != null) {
                    text.enabled = true;
                    text.alpha = 1;
                    text.text = ObjectiveManager.Instance.objectiveText;
                }
            }
        }
    }

    private void RemoveCurrentPrompt() {
        currentPrompt = null;
        
        text.DOFade(0, timeToFadeAway).OnComplete(() => {
            promptActive = false;
            text.enabled = false;
        });
    }

    private void ShowNextPrompt() {
        // pop from queue
        currentPrompt = queuedPrompts[0];
        queuedPrompts.RemoveAt(0);

        // show text
        text.enabled = true;
        text.text = currentPrompt.promptText;
        text.alpha = 1;

        // set state
        promptActive = true;
    }

    public void QueuePrompt(string controlPromptName) {
        bool foundPrompt = false;
        int currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        for (var i = 0; i < allPrompts.Count; i++)
        {
            var prompt = allPrompts[i];
            if (prompt.name == controlPromptName && prompt.day == currentDay)
            {
                queuedPrompts.Add(prompt);
                allPrompts.RemoveAt(i);
                i--;
                foundPrompt = true;
            }
            else if (foundPrompt) {
                if (prompt.queueImmediately) {
                    queuedPrompts.Add(prompt);
                    allPrompts.RemoveAt(i);
                    i--;
                }
                else break;
            }
        }
    }

    public void FinishPrompt(string controlPromptName)
    {
        if (currentPrompt == null) return;
        if (currentPrompt.name == controlPromptName) {
            RemoveCurrentPrompt();
        }
    }

    private void OnEnable() {
        // set finish conditions
        inputActions.Gameplay.Look.performed += OnLook;
        inputActions.Gameplay.Move.performed += OnMove;
        inputActions.Gameplay.Run.performed += OnSprint;
        inputActions.Gameplay.Crouch.performed += OnCrouch;
        inputActions.Gameplay.Map.performed += OnMap;
        inputActions.Gameplay.Flashlight.performed += OnFlashlight;
    }

    private void OnDisable() {
        // remove callback functions
        inputActions.Gameplay.Look.performed -= OnLook;
        inputActions.Gameplay.Move.performed -= OnMove;
        inputActions.Gameplay.Run.performed -= OnSprint;
        inputActions.Gameplay.Map.performed -= OnMap;
        inputActions.Gameplay.Flashlight.performed -= OnFlashlight;
    }

    private void OnLook(InputAction.CallbackContext context) {
        FinishPrompt("Look");
    }
    private void OnMove(InputAction.CallbackContext context) {
        FinishPrompt("Move");
    }
    private void OnSprint(InputAction.CallbackContext context) {
        FinishPrompt("Sprint");
    }
    private void OnCrouch(InputAction.CallbackContext context) {
        FinishPrompt("Crouch");
    }
    private void OnMap(InputAction.CallbackContext context) {
        FinishPrompt("Map");
    }
    private void OnFlashlight(InputAction.CallbackContext context) {
        FinishPrompt("Flashlight");
    }
}

[Serializable]
public class Prompt
{
    public string name;
    [TextArea] public string promptText;

    [Space] public bool queueImmediately;
    public int day = 1;
}