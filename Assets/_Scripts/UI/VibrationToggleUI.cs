using UnityEngine;
using UnityEngine.UI;

public class VibrationToggleUI : MonoBehaviour
{
    public Button vibrationButton;
    public Image vibrationIcon;
    public Sprite iconOn;
    public Sprite iconOff;

    void Start()
    {
        UpdateIcon();
        vibrationButton.onClick.AddListener(ToggleVibration);
    }

    void ToggleVibration()
    {
        bool newState = !VibrationManager.Ins.IsVibrationOn();
        VibrationManager.Ins.SetVibration(newState);
        UpdateIcon();
    }

    void UpdateIcon()
    {
        //bool isOn = VibrationManager.Ins.IsVibrationOn();
        //vibrationIcon.sprite = isOn ? iconOn : iconOff;
    }
}
