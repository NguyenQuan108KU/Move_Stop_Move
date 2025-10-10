using UnityEngine;
using UnityEngine.UI;

public class SoundToggleUI : MonoBehaviour
{
    public Button soundButton;
    public Image soundIcon;
    public Sprite iconOn;
    public Sprite iconOff;

    void Start()
    {
        UpdateIcon();
        soundButton.onClick.AddListener(ToggleSound);
    }

    void ToggleSound()
    {
        bool newState = !AudioManager.Ins.IsMuted();
        AudioManager.Ins.SetMute(newState);
        UpdateIcon();
    }

    void UpdateIcon()
    {
        bool isMuted = AudioManager.Ins.IsMuted();
        soundIcon.sprite = isMuted ? iconOff : iconOn;
    }
}
