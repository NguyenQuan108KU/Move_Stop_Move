using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Ins;
    public bool isLoaded = false;
    public GameSave gameSave;
    public GameSave gameSave_BackUp;

    private void Awake()
    {
        Init();
        LoadData();
    }

    void OnApplicationPause(bool pause)
    {
        if (pause == true)
            SaveGame();
    }
    void OnApplicationQuit() { SaveGame(); }

    void Reset()
    {
        gameSave.isNew = true;
    }

    public void Init()
    {
        if (Ins == null)
        {
            Ins = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void LoadData()
    {
        if (isLoaded == false)
        {
            if (PlayerPrefs.HasKey("GameSave"))
                gameSave = JsonUtility.FromJson<GameSave>(PlayerPrefs.GetString("GameSave"));
            if (gameSave.isNew)
            {
                InitData();
            }
            isLoaded = true;
        }
    }

    public void SaveGame()
    {
        try
        {
            if (!isLoaded)
                return;
            if (gameSave == null)
            {
                if (gameSave_BackUp != null)
                {
                    gameSave = gameSave_BackUp;
                    Debug.LogError("gameSave bị null , backup thành công ");
                }
                else
                {
                    InitData();
                    Debug.LogError("gameSave bị null , backup không thành công . Reset data");
                }
            }
            gameSave_BackUp = gameSave;
            PlayerPrefs.SetString("GameSave", JsonUtility.ToJson(gameSave));
            PlayerPrefs.Save();
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi LoadData:" + ex);
        }
    }

    void InitData()
    {
        gameSave = new GameSave();
        gameSave.isNew = false;
    }

    public int GetLevel()
    {
        return gameSave.level;
    }

    public void SaveLevel(int level)
    {
        gameSave.level = level;
        SaveGame();
    }

    public void UpdateSoundVolume(float volume)
    {
        gameSave.soundVolume = volume;
        SaveGame();
    }
    public void UpdateMusicVolume(float volume)
    {
        gameSave.musicVolume = volume;
        SaveGame();
    }

    public void UpdateVibrateAmount(float amount)
    {
        gameSave.vibrateAmount = amount;
        SaveGame();
    }

    public void UpdateCoin(int coin)
    {
        gameSave.coin += coin;
        SaveGame();
    }

    public int GetLevelId()
    {
        int levelCur = GetLevel();
        return levelCur;
    }
}
