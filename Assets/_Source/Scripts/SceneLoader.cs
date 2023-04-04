using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneLoader", menuName = "SceneLoader", order = 0)]
public class SceneLoader : ScriptableObject {
    public SceneReference introCutscene;
    public SceneReference day1Scene;
    public SceneReference day2Scene;
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

        if (currentDay == 1) {
            SceneManager.LoadScene(day1Scene.ScenePath);
        }
        else {
            SceneManager.LoadScene(day2Scene.ScenePath);
        }
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