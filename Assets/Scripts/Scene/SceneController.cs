using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadSceneMenu()
    {
        Time.timeScale = 1.0f;
        Scene currentScene = SceneManager.GetActiveScene();
        int sceneIndex = currentScene.buildIndex;
        if(sceneIndex == 1)
            SceneManager.LoadScene(1);
        if(sceneIndex == 2)
            SceneManager.LoadScene(2);
    }
    public void BackSceneMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void LoadLevel1() => SceneManager.LoadScene(1);
    public void LoadSceneCity() => SceneManager.LoadScene(2);
}
