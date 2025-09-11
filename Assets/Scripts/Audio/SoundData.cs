using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "soundData", menuName = "Data/Sound Data")]
public class SoundData : MonoBehaviour
{
    public SoundName soundName;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1f;
    public bool loop;

    public enum SoundName
    {
        Music_Main,//Nhạc chín
        Button_Click,//Âm thanh click
        Confetti,//Âm thanh pháo hoa
        GameFail,//Âm thanh thua
        GameWin,//Âm thanh thắng
        BreakTile,
        Clear,
        AddGrid,
        Print,
        Twist,
        UnlockSlot,
        WoolInvalid,
        WoolUp,
        Magnet,
        CoinFly,
    }
}
