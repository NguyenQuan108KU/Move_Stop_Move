using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppLovinMax;

// Khai báo enum ở đây (ngoài class)
public enum RewardType
{
    None,
    Pants,
    Hat,
    Skin,
    Shield
}

public class AdsController : MonoBehaviour
{
    public static AdsController Instance;
    private RewardType currentReward = RewardType.None;
    public HairManager hairReward;
    public PantsManager pantReward;
    public ShieldManager shieldReward;
    public ClothesManager clotheReward;

#if UNITY_ANDROID
    private string interstitialAdUnitId = "08a4c2ee787148e7"; // Android Interstitial
    private string rewardedAdUnitId = "cfce02e40fcc205d";   // Android Rewarded
    private string bannerAdUnitId = "867edadc74688fae";   // Android Banner
#elif UNITY_IOS
    private string interstitialAdUnitId = "d5e6f48cc57a3768"; // iOS Interstitial
    private string rewardedAdUnitId    = "27d9b6b0a66055c4";  // iOS Rewarded
    private string bannerAdUnitId      = "94a79256a48fa6a8";  // iOS Banner
#endif

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

    // ======================= INIT =======================
    public void InitializeAds()
    {
        InitInterstitialCallbacks();
        InitRewardedCallbacks();
        InitBanner();

        LoadInterstitial();
        LoadRewarded();
    }

    // ======================= INTERSTITIAL =======================
    private void InitInterstitialCallbacks()
    {
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += (adUnitId, adInfo) =>
        {
        };

        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += (adUnitId, errorInfo) =>
        {
            Invoke("LoadInterstitial", 3f); // Thử load lại sau 3s
        };

        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += (adUnitId, adInfo) =>
        {
        };

        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += (adUnitId, adInfo) =>
        {
            LoadInterstitial(); // Load lại cho lần sau
        };

        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += (adUnitId, errorInfo, adInfo) =>
        {
            LoadInterstitial();
        };
    }

    public void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(interstitialAdUnitId);
    }

    public void ShowInterstitial()
    {
        if (MaxSdk.IsInterstitialReady(interstitialAdUnitId))
        {
            MaxSdk.ShowInterstitial(interstitialAdUnitId);
        }
        else
        {
            LoadInterstitial();
        }
    }

    // ======================= REWARDED =======================
    private void InitRewardedCallbacks()
    {
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += (adUnitId, adInfo) =>
        {
        };

        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += (adUnitId, errorInfo) =>
        {
            Invoke("LoadRewarded", 3f);
        };

        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += (adUnitId, adInfo) =>
        {
        };

        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += (adUnitId, errorInfo, adInfo) =>
        {
            LoadRewarded();
        };

        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += (adUnitId, adInfo) =>
        {
   
            LoadRewarded();
        };

        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += (adUnitId, reward, adInfo) =>
        {
            GiveReward(currentReward);
        };
    }

    public void LoadRewarded()
    {
        MaxSdk.LoadRewardedAd(rewardedAdUnitId);
    }

    public void ShowRewarded(RewardType rewardType)
    {
        currentReward = rewardType;
        if (MaxSdk.IsRewardedAdReady(rewardedAdUnitId))
        {
            MaxSdk.ShowRewardedAd(rewardedAdUnitId);
        }
        else
        {
            LoadRewarded();
        }
    }
    public void ShowRewardPants()
    {
        ShowRewarded(RewardType.Pants);
    }

    public void ShowRewardHat()
    {
        ShowRewarded(RewardType.Hat);
    }

    public void ShowRewardSkin()
    {
        ShowRewarded(RewardType.Skin);
    }

    public void ShowRewardShield()
    {
        ShowRewarded(RewardType.Shield);
    }

    private void GiveReward(RewardType rewardType)
    {
        switch (rewardType)
        {
            case RewardType.Pants:
                pantReward.BuyPantByAds();
                break;
            case RewardType.Hat:
                hairReward.BuyHairByAds();
                break;
            case RewardType.Skin:
                clotheReward.BuyClothesByAds();
                break;
            case RewardType.Shield:
                shieldReward.BuyProtectByAds();
                break;
            default:
                Debug.Log("Không có reward.");
                break;
        }

        currentReward = RewardType.None; // reset
    }
    // ======================= BANNER =======================
    private void InitBanner()
    {
        MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);
        MaxSdk.SetBannerExtraParameter(bannerAdUnitId, "adaptive_banner", "true");
        MaxSdk.HideBanner(bannerAdUnitId); // Mặc định ẩn
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
