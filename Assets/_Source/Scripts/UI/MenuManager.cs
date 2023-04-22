using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    // constants
    [SerializeField] UIConstants uiConstants;
    public List<MenuScreen> menuScreens;
    public UnityEvent defaultBackAction;

    private InputActions inputActions;
    
    // state
    private List<MenuScreen> menuScreenStack = new List<MenuScreen>();

    private void OnEnable()
    {
        inputActions = new InputActions();
        inputActions.Enable();

        if (menuScreens.Count > 0)
        {
            // hide all menu screens
            foreach (var screen in menuScreens)
            {
                screen.gameObject.SetActive(true);
                screen.StartOffscreen();
            }

            // show starting menu screen
            var startingScreen = menuScreens[0];
            startingScreen.StartOnscreen();

            menuScreenStack.Clear();
            menuScreenStack.Add(startingScreen);
        }
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        menuScreenStack.Clear();
    }

    private void Update()
    {
        if (inputActions.UI.Back.triggered)
        {
            BackToPreviousMenuScreen();

            if (AudioManager.Instance) AudioManager.Instance.PlayBackSound();
        }
    }

    public void OpenMenuScreen(MenuScreen screen)
    {
        // disable current menu screen
        menuScreenStack[menuScreenStack.Count - 1].PutAway();

        // push new menu screen to stack & enable
        menuScreenStack.Add(screen);
        screen.Push();
    }

    public void BackToPreviousMenuScreen()
    {
        // if at root menu screen, do default back action instead
        if (menuScreenStack.Count <= 1)
        {
            defaultBackAction?.Invoke();
            return;
        }

        // disable current menu screen & pop from stack
        menuScreenStack[menuScreenStack.Count - 1].Pop();
        menuScreenStack.RemoveAt(menuScreenStack.Count - 1);

        // enable previous menu screen
        menuScreenStack[menuScreenStack.Count - 1].BringBack();
    }

    public void NewGame() {
        PlayerPrefs.DeleteKey("CurrentDay");
        // SceneManager.LoadScene(introCutscene.ScenePath);
    }

    public void LoadCurrentDay() {
        int currentDay = PlayerPrefs.GetInt("CurrentDay", 1);

        /*if (currentDay == 1) {
            SceneManager.LoadScene(day1Scene.ScenePath);
        }
        else {
            SceneManager.LoadScene(day2Scene.ScenePath);
        }*/
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu() {
        Time.timeScale = 1;
        //SceneManager.LoadScene(mainMenuScene.ScenePath);
    }

    public void Quit() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}