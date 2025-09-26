using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsController : MonoBehaviour
{
    public static AdsController Instance;

#if UNITY_ANDROID
    private string interstitialAdUnitId = "08a4c2ee787148e7"; // Android Interstitial
    private string rewardedAdUnitId    = "cfce02e40fcc205d"; // Android Rewarded
    private string bannerAdUnitId      = "867edadc74688fae"; // Android Banner
#elif UNITY_IOS
    private string interstitialAdUnitId = "d5e6f48cc57a3768"; // iOS Interstitial
    private string rewardedAdUnitId    = "27d9b6b0a66055c4"; // iOS Rewarded
    private string bannerAdUnitId      = "94a79256a48fa6a8"; // iOS Banner
#endif
    private string interstitialAdUnitId = "08a4c2ee787148e7"; // Android Interstitial
    private string rewardedAdUnitId = "cfce02e40fcc205d"; // Android Rewarded
    private string bannerAdUnitId = "867edadc74688fae"; // Android Banner

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeAds()
    {
        // Load quảng cáo lần đầu
        LoadInterstitial();
        LoadRewarded();
        LoadBanner();
    }

    public void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interstitialAdUnitId);
    }

    public void LoadRewarded()
    {
        MaxSdk.LoadRewardedAd(rewardedAdUnitId);
    }

    public void ShowInterstitial()
    {
        if (MaxSdk.IsInterstitialReady(interstitialAdUnitId))
        {
            MaxSdk.ShowInterstitial(interstitialAdUnitId);
        }
        else
        {
            Debug.Log("Interstitial chưa sẵn sàng!");
            LoadInterstitial();
        }
    }

    public void ShowRewarded()
    {
        if (MaxSdk.IsRewardedAdReady(rewardedAdUnitId))
        {
            MaxSdk.ShowRewardedAd(rewardedAdUnitId);
        }
        else
        {
            Debug.Log("Rewarded chưa sẵn sàng!");
            LoadRewarded();
        }
    }
    public void LoadBanner()
    {
        // Tạo banner nếu chưa tạo
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

        // Đặt khoảng padding (nếu muốn tránh overlap UI, ví dụ margin 10px)
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black); // Tuỳ chỉnh màu nền
        MaxSdk.SetBannerExtraParameter(bannerAdUnitId, "adaptive_banner", "true");

        // Ẩn mặc định, chỉ show khi cần
        MaxSdk.HideBanner(bannerAdUnitId);
    }

    public void ShowBanner()
    {
        MaxSdk.ShowBanner(bannerAdUnitId);
    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(bannerAdUnitId);
    }
}
