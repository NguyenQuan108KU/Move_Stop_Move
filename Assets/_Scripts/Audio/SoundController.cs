using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController : MonoBehaviour
{
    public AudioSource audioSource;
    public bool isMusic;//Có phải là âm nhạc hay không
    public SoundData soundData { get; private set; }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false; // ✅ tắt ngay từ Awake
        audioSource.clip = null;          // AudioSource rỗng, không tự phát
    }

    public void PlaySound(SoundData.SoundName soundName)
    {
        if (AudioManager.Ins == null || !AudioManager.Ins.CanPlaySound(soundName)) return;
        soundData = AudioManager.Ins.GetSoundData(soundName);
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (soundData != null && soundData.audioClip != null)
        {
            if (DataManager.Ins != null)
                audioSource.volume = soundData.volume * (isMusic ? DataManager.Ins.gameSave.musicVolume : DataManager.Ins.gameSave.soundVolume);
            else
                audioSource.volume = soundData.volume;
            audioSource.clip = soundData.audioClip;
            audioSource.loop = soundData.loop;
            audioSource.Play();
        }
        if (AudioManager.Ins != null)
            AudioManager.Ins.AddSoundList(this);
    }

    public void PauseSound()
    {
        if (audioSource != null && audioSource.isPlaying)
            audioSource.Pause();
    }

    public bool IsSoundPlay()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource.clip == null) return false;
        return audioSource.isPlaying;

    }

    public void UpdateVolume()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        audioSource.volume = soundData.volume * (isMusic ? DataManager.Ins.gameSave.musicVolume : DataManager.Ins.gameSave.soundVolume);
        if (isMusic && AudioManager.Ins.isChangeTempVolume)
        {
            audioSource.volume = audioSource.volume / 2;
        }
    }
}
