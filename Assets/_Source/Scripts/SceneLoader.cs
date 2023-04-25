using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneLoader", menuName = "SceneLoader", order = 0)]
public class SceneLoader : ScriptableObject {
    public SceneReference introCutscene;
    public List<SceneReference> dayScenes;
    public SceneReference mainMenuScene;
    public SceneReference shopScene;
    public SceneReference cutsceneScene;
    
    public void NewGame() {
        Time.timeScale = 1;
        PlayerPrefs.DeleteKey("CurrentDay");
        SceneManager.LoadScene(introCutscene.ScenePath);
    }

    public void LoadCurrentDay() {
        Time.timeScale = 1;
        int currentDay = PlayerPrefs.GetInt("CurrentDay", 1);
        int index = currentDay - 1;
        if (index >= dayScenes.Count) index = dayScenes.Count - 1;

        SceneManager.LoadScene(dayScenes[index].ScenePath);
    }

    public void LoadShop() {
        Time.timeScale = 1;
        SceneManager.LoadScene(shopScene.ScenePath);
    }

    public void LoadCutscene() {
        Time.timeScale = 1;
        SceneManager.LoadScene(cutsceneScene.ScenePath);
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuScene.ScenePath);
    }

    public void Quit() {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}