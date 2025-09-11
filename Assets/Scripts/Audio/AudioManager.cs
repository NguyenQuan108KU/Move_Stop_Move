using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Ins { get; private set; }
    public List<SoundController> soundList;
    public List<SoundController> musicList;

    [Header("--------------------Data---------------")]
    public List<SoundData> soundDatas;
    public SoundController music;
    public SoundController soundButtonClick;
    public bool isChangeTempVolume;
    Dictionary<SoundData.SoundName, float> lastPlayTimeDic;

    void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Ins = this;
            DontDestroyOnLoad(gameObject);
        }
        lastPlayTimeDic = new Dictionary<SoundData.SoundName, float>();
    }

    public bool CanPlaySound(SoundData.SoundName soundName)
    {
        if (lastPlayTimeDic.ContainsKey(soundName))
        {
            if (Time.time >= lastPlayTimeDic[soundName])
            {
                float length = GetSoundData(soundName).audioClip.length;
                lastPlayTimeDic[soundName] = Time.time + length / 1.6f;
                return true;
            }
            else { return false; }
        }
        return true;
    }

    public void AddSoundList(SoundController soundController)
    {
        if (soundController.isMusic && !musicList.Contains(soundController))
        {
            musicList.Add(soundController);
        }
        else if (!soundController.isMusic && !soundList.Contains(soundController))
            soundList.Add(soundController);
    }

    public void PlayMusic(SoundData.SoundName soundName)
    {
        if ((music.soundData != null && music.soundData.soundName == soundName) || (music.audioSource.clip != null && music.audioSource.clip.name == GetSoundData(soundName).audioClip.name)) return;
        music.PlaySound(soundName);
    }

    public void PlaySound_ButtonClick()
    {
        soundButtonClick.PlaySound(SoundData.SoundName.Button_Click);
        if (DataManager.Ins != null && DataManager.Ins.gameSave.vibrateAmount > 0)
            Vibration.VibrateButton();
    }

    public void PauseAllSound()
    {
        for (int i = soundList.Count - 1; i >= 0; i--)
        {
            if (soundList[i] == null)
            {
                soundList.RemoveAt(i);
                continue;
            }
            soundList[i].PauseSound();
        }
    }

    //Hàm giảm âm lượng music game theo thời gian
    //Nếu < 0 thì tắt lun
    public void DelayMusic(float timeDelay)
    {
        music.audioSource.volume = music.audioSource.volume / 2;
        isChangeTempVolume = true;
        if (timeDelay > 0)
        {
            DOVirtual.DelayedCall(timeDelay, () => {
                isChangeTempVolume = false;
                UpdateVolume();
            });
        }
        else
            isChangeTempVolume = false;
    }

    public void UpdateVolume()
    {
        for (int i = soundList.Count - 1; i >= 0; i--)
        {
            if (soundList[i] == null)
            {
                soundList.RemoveAt(i);
                continue;
            }
            soundList[i].UpdateVolume();
        }
        for (int i = musicList.Count - 1; i >= 0; i--)
        {
            if (musicList[i] == null)
            {
                musicList.RemoveAt(i);
                continue;
            }
            musicList[i].UpdateVolume();
        }
    }

    public SoundData GetSoundData(SoundData.SoundName soundName)
    {
        foreach (SoundData soundData in soundDatas)
        {
            if (soundData.soundName == soundName)
                return soundData;
        }
        Debug.LogWarning("Don't have sound " + soundName);
        return soundDatas[0];
    }
}
