using AppsFlyerSDK;
using System.Collections.Generic;
using UnityEngine;

public class AppsFlyerManager : MonoBehaviour
{
    [SerializeField] private int currentLevel = 1;

    void Start()
    {
#if UNITY_IOS
        AppsFlyer.initSDK("xEuExYvWKi7kRUbpNFkpFsW", "6744522051");
#elif UNITY_ANDROID
        AppsFlyer.initSDK("xEuExYvWKi7kRUbpNFkpFsW", "com.pub.um.quick.game100.monsters");
#endif
        AppsFlyer.startSDK();
        Debug.Log("AppsFlyer SDK initialized");

        // Sau khi init xong thì bắn event
        Dictionary<string, string> eventValues = new Dictionary<string, string>();
        eventValues.Add("level", currentLevel.ToString());

        AppsFlyer.sendEvent("start_game", eventValues);

        Debug.Log($"AppsFlyer Event sent: start_game | level = {currentLevel}");
    }
}
