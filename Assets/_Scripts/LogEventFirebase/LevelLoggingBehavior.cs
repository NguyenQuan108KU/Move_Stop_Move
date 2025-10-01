using Firebase.Analytics;
using UnityEngine;

public class LevelLoggingBehavior : MonoBehaviour
{
    [SerializeField] private int currentLevel = 1; // Gán level hiện tại trong Inspector

    void Start()
    {
        // Log custom event khi bắt đầu game
        FirebaseAnalytics.LogEvent(
            "start_game",
            new Parameter("level", currentLevel)
        );

        Debug.Log($"Firebase Event: start_game | level = {currentLevel}");
    }
}
