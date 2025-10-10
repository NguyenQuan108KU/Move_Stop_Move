using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    void Start()
    {
        if (PlayerPrefs.GetInt("HasPlayedIntro", 0) == 1)
        {
            // Nếu đã xem intro thì bỏ qua luôn
            SceneManager.LoadScene(1);
        }
        else
        {
            // Lần đầu → set cờ để lần sau khỏi xem intro nữa
            PlayerPrefs.SetInt("HasPlayedIntro", 1);
            PlayerPrefs.Save();
        }
    }
}
