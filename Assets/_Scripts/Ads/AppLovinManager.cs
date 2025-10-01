using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppLovinManager : MonoBehaviour
{
    private void Awake()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            AdsController.Instance.InitializeAds();
            AdsController.Instance.ShowBanner();
        };
        MaxSdk.InitializeSdk();
    }

}
