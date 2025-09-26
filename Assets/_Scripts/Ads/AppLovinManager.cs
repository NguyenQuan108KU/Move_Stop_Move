using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppLovinMax; 

public class AppLovinManager : MonoBehaviour
{
    private void Awake()
    {
        // Khởi tạo SDK
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            Debug.Log("AppLovin MAX SDK Initialized");

            // Sau khi SDK init xong thì load quảng cáo
            AdsController.Instance.InitializeAds();
            //AdsController.Instance.ShowBanner();
        };

        MaxSdk.InitializeSdk();
    }
}
