using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;

public class FirebaseInit : MonoBehaviour
{

    //FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>{
    //    var app = FirebaseApp.DefaultInstance;
    //});
    public static FirebaseInit Instance { get; private set; }
    public static bool IsReady { get; private set; } = false;

    private static bool hasLoggedStartEvent = false; // đảm bảo event chỉ bắn 1 lần
    [SerializeField] private int startLevel = 1;     // Cho phép gán level trong Inspector

    private void Awake()
    {
        // Đảm bảo chỉ có một instance tồn tại
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitFirebase();
    }

    private async void InitFirebase()
    {
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependencyStatus == DependencyStatus.Available)
        {
            IsReady = true;
            Debug.Log("Firebase initialized successfully.");

            LogStartGameEventOnce(); // Bắn event start_game đúng 1 lần
        }
        else
        {
            Debug.LogError($"Firebase init failed: {dependencyStatus}");
        }
    }

    private void LogStartGameEventOnce()
    {
        if (hasLoggedStartEvent)
            return;

        hasLoggedStartEvent = true;

        // Bắn event có kèm tham số level
        FirebaseAnalytics.LogEvent(
            "start_game",
            new Parameter("level", startLevel)
        );

        Debug.Log($"Firebase Event sent: start_game | level = {startLevel}");
    }
}
