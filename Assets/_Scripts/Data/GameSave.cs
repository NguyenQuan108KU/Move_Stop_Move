using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class GameSave
{
    public bool isNew;
    public int coin;
    public int heart;
    public int amountBoosterAddSlotTemp, amountBoosterEmptySlotTemp, amountBoosterHammerBreak, amountBoosterMagnet;
    public bool isDoneTutGameplay;
    public bool isUnlockBoosterAddSlotTemp, isUnlockBoosterEmptySlotTemp, isUnlockBoosterHammerBreak, isUnlockBoosterMagnet;
    public int level;
    public int levelStart;
    public int levelEnd;
    [Space]
    public string idPant;       // id quần 
    public string idHat;
    public string idShield;
    public string idSkin;
    public string idWeapon;
    public List<string> objectsBought;  // danh sách các đồ vật đã mua
    public float soundVolume;
    public float musicVolume;
    public float vibrateAmount;
    public int daysPlayed;
    public int sessionStart;
    public bool isNoAds;
    public string heartTime;

    public GameSave()
    {
        idPant = "Pants_7";
        idHat = "Hats_7";
        idShield = "Shiled_2";
        idSkin = "Skin_2";
        idWeapon = "";
        objectsBought = new List<string>();
        isNew = true;
        level = 0;
        levelStart = -1;
        levelEnd = -1;
        soundVolume = 1;
        musicVolume = 0;
        vibrateAmount = 1;
        coin = 0;
        isNoAds = false;
        sessionStart = 0;
        daysPlayed = 0;
        amountBoosterAddSlotTemp = amountBoosterEmptySlotTemp = amountBoosterHammerBreak = amountBoosterMagnet = 1;
    }
}
