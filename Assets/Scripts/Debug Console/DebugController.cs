using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour {
    // components
    public static DebugController Instance;
    private InputActions inputActions;
    public GameObject consolePanel;
    public TextMeshProUGUI inputText;
    
    // state
    private bool consoleVisible;
    private string input;
    private Vector2 scroll;

    // commands
    public List<object> commandList;
    public static DebugCommand KILL_PLAYER;
    public static DebugCommand END_DAY;
    public static DebugCommand<int> SET_SEEDS;
    public static DebugCommand SET_DAY;
    public static DebugCommand SET_EVENING;
    public static DebugCommand SET_NIGHT;


    private void Awake() {
        Instance = this;
        inputActions = new InputActions();
        inputActions.Enable();
    }

    private void Start() {
        KILL_PLAYER = new DebugCommand("kill_player", "Kills player.", "kill_player", () => {
            GameManager.Instance.GameOver(false);
        });
        END_DAY = new DebugCommand("end_day", "Ends the current day.", "end_day", () => {
            GameManager.Instance.GameOver(true);
        });
        SET_SEEDS = new DebugCommand<int>("set_seeds", "Sets the number of seeds", "set_seeds <seed_number>", x => {
            ResourceManager.Instance.SetSeeds(x);
        });
        SET_DAY = new DebugCommand("set_day", "Sets the time to day.", "set_day", () => {
            DayManager.Instance.SetDay();
        });
        SET_EVENING = new DebugCommand("set_evening", "Sets the time to evening.", "set_evening", () => {
            DayManager.Instance.SetEvening();
        });
        SET_NIGHT = new DebugCommand("set_night", "Sets the time to night.", "set_night", () => {
            DayManager.Instance.SetNight();
        });
        commandList = new List<object> {
            KILL_PLAYER,
            END_DAY,
            SET_SEEDS,
            SET_DAY,
            SET_EVENING,
            SET_NIGHT
        };
        
        // start disabled
        consoleVisible = false;
        consolePanel.SetActive(false);
    }

    private void OnEnable() {
        Keyboard.current.onTextInput += OnTextInput;
    }
    private void OnDisable() {
        Keyboard.current.onTextInput -= OnTextInput;
    }

    public void ToggleDebug() {
        SetVisible(!consoleVisible);
    }

    private void SetVisible(bool value) {
        consoleVisible = value;
        consolePanel.SetActive(value);
        ClearText();
        
        if(value) GameManager.Instance.Pause(true);
        else GameManager.Instance.Resume(true);
    }

    public void Return() {
        if (consoleVisible) {
            HandleInput();
            ClearText();
        }
    }

    private void Update() {
        if (!consoleVisible) return;
        
        if(Keyboard.current.backspaceKey.isPressed) ClearText();

        if (inputActions.UI.Back.triggered) {
            SetVisible(false);
        }
    }

    private void OnTextInput(char ch) {
        if (ch.Equals('`')) return;
        
        input += ch;
        inputText.text = input;
    }

    private void ClearText() {
        input = "";
        inputText.text = input;
    }

    private void HandleInput() {
        string[] properties = input.Split(' ');
        for (int i = 0; i < commandList.Count; i++) {
            DebugCommandBase commandBase = commandList[i] as DebugCommandBase;
            if (input.Contains(commandBase.commandID)) {
                if (commandList[i] as DebugCommand != null) {
                    // cast to this type and invoke the command
                    (commandList[i] as DebugCommand).Invoke();
                }
                else if (commandList[i] as DebugCommand<int> != null) {
                    (commandList[i] as DebugCommand<int>).Invoke(int.Parse(properties[1]));
                }
            }
        }
    }
}
