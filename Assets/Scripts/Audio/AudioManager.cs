using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource[] sfx;
    public AudioSource[] bgm;
    public bool isPlayerBGM;
    public bool isWinner;
    public int BGMIndex;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    private void Update()
    {
        if (!isPlayerBGM)
        {
            StopBGM();
        }
        else
        {
            if (!bgm[BGMIndex].isPlaying)
            {
                PlayerBGM(BGMIndex);
            }
        }
    }
    public void PlayerSFX(int _sfxIndex)
    {
        if (_sfxIndex < sfx.Length)
        {
            sfx[_sfxIndex].Play();
        }
    }
    public void StopSFX(int _sfxIndex) => sfx[_sfxIndex].Stop();
    public void PlayerBGM(int _bgmIndex)
    {
        StopBGM();
        bgm[_bgmIndex].Play();
    }
    public void StopBGM()
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            bgm[i].Stop();
        }
    }
    public void OffAudio() => isPlayerBGM = false;

}
