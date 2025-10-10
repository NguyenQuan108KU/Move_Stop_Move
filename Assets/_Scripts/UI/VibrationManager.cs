using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager Ins { get; private set; }

    void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(gameObject);
            return;
        }
        Ins = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Vibrate()
    {
        if (!IsVibrationOn()) return;

#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }

    public void SetVibration(bool isOn)
    {
        PlayerPrefs.SetInt("Vibration", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsVibrationOn()
    {
        return PlayerPrefs.GetInt("Vibration", 1) == 1; // mặc định bật
    }
}
