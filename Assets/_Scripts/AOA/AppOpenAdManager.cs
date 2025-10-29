using System;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.SceneManagement; // thêm ở đầu file nếu chưa có

public class AppOpenAdManager : MonoBehaviour
{
    public static AppOpenAdManager Instance { get; private set; }

    [Header("AdMob")]
    [Tooltip("Dùng test ID khi test. Thay bằng Ad Unit ID thật khi publish.")]
    [SerializeField] private string adUnitId = "ca-app-pub-6409857233709298/8480480070";

    [Header("Behavior")]
    [Tooltip("Nếu true sẽ cố show ad ngay khi load xong lần đầu (thường dùng cho splash).")]
    public bool showOnLoad = true;

    // Ad và trạng thái
    private AppOpenAd appOpenAd = null;
    private DateTime loadTime;
    private bool isShowingAd = false;
    private bool hasShownAdThisSession = false; // đảm bảo show 1 lần / session
    public static bool IsOtherAdShowing = false;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob initialized");
            LoadAppOpenAd();
        });
    }

    /// <summary>
    /// Load App Open Ad
    /// </summary>
    public void LoadAppOpenAd()
    {
        Debug.Log("Loading App Open Ad...");
        AdRequest request = new AdRequest();
        AppOpenAd.Load(adUnitId, request, (ad, error) =>
        {
            if (error != null)
            {
                Debug.LogError($"Failed to load AppOpenAd: {error.GetMessage()}");
                return;
            }

            this.appOpenAd = ad;
            this.loadTime = DateTime.UtcNow;
            Debug.Log("AppOpenAd loaded successfully.");

            // Register callbacks
            appOpenAd.OnAdFullScreenContentOpened += OnAdOpened;
            appOpenAd.OnAdFullScreenContentClosed += OnAdClosed;
            appOpenAd.OnAdFullScreenContentFailed += OnAdFailedToShow;

            // Nếu muốn show ngay sau khi load (ví dụ splash), làm ở đây
            if (showOnLoad && !hasShownAdThisSession)
            {
                ShowAdIfAvailable();
            }
        });
    }

    private bool IsAdAvailable()
    {
        // Thưc tế: Google khuyến nghị ad Open hợp lệ ~4 giờ
        return appOpenAd != null && (DateTime.UtcNow - loadTime).TotalHours < 4;
    }

    /// <summary>
    /// Show ad nếu có và chưa show session này
    /// </summary>
    public void ShowAdIfAvailable()
    {
        if (isShowingAd || IsOtherAdShowing)
        {
            Debug.Log("AOA: Đang hiển thị quảng cáo khác hoặc AOA đang mở.");
            return;
        }

        if (!IsAdAvailable())
        {
            Debug.Log("AOA: Chưa có quảng cáo sẵn, đang load lại...");
            LoadAppOpenAd();
            return;
        }

        if (hasShownAdThisSession)
        {
            Debug.Log("AOA: Đã hiển thị trong session này, bỏ qua.");
            return;
        }

        Debug.Log("AOA: Hiển thị quảng cáo...");
        isShowingAd = true;
        appOpenAd.Show();
        hasShownAdThisSession = true;
    }


    private void OnAdOpened()
    {
        Debug.Log("AppOpenAd opened.");
    }

    private void OnAdClosed()
    {
        Debug.Log("AppOpenAd closed.");
        isShowingAd = false;

        // Cleanup old ad reference
        if (appOpenAd != null)
        {
            appOpenAd.OnAdFullScreenContentOpened -= OnAdOpened;
            appOpenAd.OnAdFullScreenContentClosed -= OnAdClosed;
            appOpenAd.OnAdFullScreenContentFailed -= OnAdFailedToShow;
            appOpenAd = null;
        }

        // Load next ad for future resume
        LoadAppOpenAd();
    }

    private void OnAdFailedToShow(AdError error)
    {
        Debug.LogError("AppOpenAd failed to show: " + error.GetMessage());
        isShowingAd = false;
        // Tự động thử load lại
        LoadAppOpenAd();
    }

    // Khi app chuyển từ background -> foreground (cả Android & iOS)
    private void OnApplicationPause(bool pause)
    {
        if (!pause) // resumed
        {
            Debug.Log("Application resumed (OnApplicationPause false). Try show AOA.");
            ShowAdIfAvailable();
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Debug.Log("Application focused (OnApplicationFocus true). Try show AOA.");
            ShowAdIfAvailable();
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu") // đổi "Menu" đúng với tên scene bạn dùng
        {
            hasShownAdThisSession = false;
            ShowAdIfAvailable();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
