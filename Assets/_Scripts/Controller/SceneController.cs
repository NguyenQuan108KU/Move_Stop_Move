using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Load lại scene menu hiện tại
    public void LoadSceneMenu(){
        Time.timeScale = 1.0f;                                  // Reset timeScale khi load lại
        Scene currentScene = SceneManager.GetActiveScene();     // Lấy scene hiện tại
        int sceneIndex = currentScene.buildIndex;               // Lấy build index
        if (sceneIndex == 2)
            SceneManager.LoadScene(2);                          // Load scene 1 nếu đang ở scene 1
        if (sceneIndex == 3)
            SceneManager.LoadScene(3);                          // Load scene 2 nếu đang ở scene 2
    }

    public void NextScene() => SceneManager.LoadScene(4);

    // Quay về scene menu chính (index 0)
    public void BackSceneMenu() => SceneManager.LoadScene(1);

    // Load scene Level 1
    public void LoadLevel1() => SceneManager.LoadScene(2);

    // Load scene ZombieCity
    public void LoadSceneCity() => SceneManager.LoadScene(3);
    public void LoadSceneByName()
    {
        int level = DataManager.Ins.gameSave.level;
        string sceneName = "Level" + level;
        SceneManager.LoadScene(sceneName);
    }
}
