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
    private List<DebugCommandBase> commandList;
    private static DebugCommand KILL_PLAYER;
    private static DebugCommand END_DAY;
    private static DebugCommand<int> SET_SEEDS;
    private static DebugCommand SET_DAY;
    private static DebugCommand SET_EVENING;
    private static DebugCommand SET_NIGHT;
    private static DebugCommand<bool> SET_DEAF;
    private static DebugCommand<bool> SET_BLIND;


    private void Awake() {
        Instance = this;
        inputActions = new InputActions();
        inputActions.Enable();
    }

    private void Start() {
        KILL_PLAYER = new DebugCommand("kill_player", "Kills player.", "kill_player",
            () => { GameManager.Instance.GameOver(false); });
        END_DAY = new DebugCommand("end_day", "Ends the current day.", "end_day",
            () => { GameManager.Instance.GameOver(true); });
        SET_SEEDS = new DebugCommand<int>("set_seeds", "Sets the number of seeds", "set_seeds <seed_number>",
            x => { ResourceManager.Instance.SetSeeds(x); });
        SET_DAY = new DebugCommand("set_day", "Sets the time to day.", "set_day",
            () => { DayManager.Instance.SetDay(); });
        SET_EVENING = new DebugCommand("set_evening", "Sets the time to evening.", "set_evening",
            () => { DayManager.Instance.SetEvening(); });
        SET_NIGHT = new DebugCommand("set_night", "Sets the time to night.", "set_night",
            () => { DayManager.Instance.SetNight(); });
        SET_DEAF = new DebugCommand<bool>("set_deaf", "Sets Torbalan deaf state.", "set_deaf <bool>", value => {
            TorbalanHearing.Instance.SetDeaf(value);
        });
        SET_BLIND = new DebugCommand<bool>("set_blind", "Sets Torbalan blind state.", "set_blind <bool>", value => {
            TorbalanVision.Instance.SetBlind(value);
        });

    commandList = new List<DebugCommandBase> {
            KILL_PLAYER,
            END_DAY,
            SET_SEEDS,
            SET_DAY,
            SET_EVENING,
            SET_NIGHT,
            SET_DEAF,
            SET_BLIND,
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
        foreach (DebugCommandBase commandBase in commandList) {
            if (input.Contains(commandBase.commandID)) {
                switch (commandBase) {
                    case DebugCommand command:
                        command.Invoke();
                        break;
                    case DebugCommand<int> commandInt: {
                        int argInt = int.Parse(properties[1]);
                        commandInt.Invoke(argInt);
                        break;
                    }
                    case DebugCommand<bool> commandBool: {
                        bool argBool = bool.Parse(properties[1]);
                        commandBool.Invoke(argBool);
                        break;
                    }
                }
            }
        }
    }
}
