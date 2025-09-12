using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClothesManager : MonoBehaviour
{
    [Header("-------------------Skin Data-------------------")]
    public SkinDatabases skinDatabases;
    public ClothesSet[] clothesSet;
    public GameObject[] hairList;
    public int[] clothesPrices;
    public GameObject hatAnchorOfSkin;
    public GameObject shieldAnchorOfSkin;
    public GameObject wingAnchorOfSkin;
    public GameObject tailAnchorOfSkin;
    public Transform[] list_anchorsOfSkin;

    public Button[] list_Buttons;
    public int currentButtonIndex;

    public GameObject ButtonBuy;
    public TextMeshProUGUI textOfButtonSelect;
    public Image imageOfButtonSelect;

    [Header("Buy clothes")]
    public int coinOfPlayer;
    public TextMeshProUGUI coinOfClothes;

    public GameObject buyByCoin;
    public GameObject buyByAds;
    public GameObject selectPaint;

    public SkinnedMeshRenderer initialShadingOfPlayer;
    public SkinnedMeshRenderer pantsOfPlayer;

    public Material materialDefaultOfPlayer;
    public HairManager hairManager;
    public ShieldManager protectManager;
    public PantsManager pantsManager;
    public bool isResetClothes;
    private void Start()
    {
        
        coinOfPlayer = PlayerPrefs.GetInt("coinMoney");
        for (int i = 0; i < list_Buttons.Length; i++)
        {
            int index = i;
            list_Buttons[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                currentButtonIndex = index;
                StateSkinOfPlayer(currentButtonIndex);
                StateOfButton(currentButtonIndex);
                RefreshActionButton();
            });
        }
    }
    public void ButtonClick()
    {
        if (textOfButtonSelect.text == "SELECT")
        {
            isResetClothes = true;
            ResetItemOfPlayer();
            hairManager.isSetHat = false;
            protectManager.isSetShield = false;
            pantsManager.isSetPant = false;
            string skinName = skinDatabases.skin[currentButtonIndex].index;
            DataManager.Ins.gameSave.idSkin = skinName;
            DataManager.Ins.SaveGame();
            SetActionButton("Unequip", Color.white);
        }
        else if (textOfButtonSelect.text == "Unequip")
        {
            isResetClothes = false;
            string skinName = skinDatabases.skin[skinDatabases.skin.Length - 1].index;
            DataManager.Ins.gameSave.idSkin = skinName;
            DataManager.Ins.SaveGame();
            SetActionButton("SELECT", new Color(1f, 221f / 255f, 0f));
        }
        SetSkinOfPlayer();
    }
    public void BuyClothes()
    {
        string skinName = skinDatabases.skin[currentButtonIndex].index;
        int coinOfHat = skinDatabases.skin[currentButtonIndex].coinOfSkin;
        if (coinOfPlayer >= coinOfHat)
        {
            coinOfPlayer -= coinOfHat;
            PlayerPrefs.SetInt("coinMoney", coinOfPlayer);
            if (!DataManager.Ins.gameSave.objectsBought.Contains(skinName))
            {
                DataManager.Ins.gameSave.objectsBought.Add(skinName);
            }
            DataManager.Ins.SaveGame();
            buyByAds.SetActive(false);
            buyByCoin.SetActive(false);
            selectPaint.SetActive(true);
        }
    }
    public void StateOfButton(int index)
    {
        string skinName = skinDatabases.skin[index].index;
        if (DataManager.Ins.gameSave.objectsBought.Contains(skinName))
        {
            buyByAds.SetActive(false);
            buyByCoin.SetActive(false);
            selectPaint.SetActive(true);
        }
        else
        {
            buyByAds.SetActive(true);
            buyByCoin.SetActive(true);
            selectPaint.SetActive(false);
        }
    }
    public void StateSkinOfPlayer(int index)
    {
        foreach (Transform anchor in list_anchorsOfSkin)
        {
            foreach (Transform child in anchor)
            {
                Destroy(child.gameObject);
            }
        }
        Debug.Log(index);
        Instantiate(skinDatabases.skin[index].hatOfSkin, hatAnchorOfSkin.transform);
        Instantiate(skinDatabases.skin[index].shieldOfSkin, shieldAnchorOfSkin.transform);
        Instantiate(skinDatabases.skin[index].wingOfSkin, wingAnchorOfSkin.transform);
        Instantiate(skinDatabases.skin[index].tailOfSkin, tailAnchorOfSkin.transform);
        initialShadingOfPlayer.material = skinDatabases.skin[index].materialOfPlayer;
        pantsOfPlayer.material = skinDatabases.skin[index].materialOfPlayer;
    }
    public void SetSkinOfPlayer()
    {
        string skinName = DataManager.Ins?.gameSave?.idSkin;
        if (string.IsNullOrEmpty(skinName)) return;

        foreach (Transform anchor in list_anchorsOfSkin)
        {
            foreach (Transform child in anchor)
            {
                Destroy(child.gameObject);
            }
        }
        for (int i = 0; i < skinDatabases.skin.Length; i++)
        {
            if (skinDatabases.skin[i].index == skinName)
            {
                Instantiate(skinDatabases.skin[i].hatOfSkin, hatAnchorOfSkin.transform);
                Instantiate(skinDatabases.skin[i].shieldOfSkin, shieldAnchorOfSkin.transform);
                Instantiate(skinDatabases.skin[i].wingOfSkin, wingAnchorOfSkin.transform);
                Instantiate(skinDatabases.skin[i].tailOfSkin, tailAnchorOfSkin.transform);
                initialShadingOfPlayer.material = skinDatabases.skin[i].materialOfPlayer;
                pantsOfPlayer.material = skinDatabases.skin[i].materialOfPlayer;
            }
        }
    }
    private void SetActionButton(string text, Color color)
    {
        if (textOfButtonSelect != null)
            textOfButtonSelect.text = text;

        if (imageOfButtonSelect != null)
            imageOfButtonSelect.color = color;
    }
    private void RefreshActionButton()
    {
        string selectedSkin = DataManager.Ins.gameSave.idSkin;
        if (string.IsNullOrEmpty(selectedSkin)) return;

        bool isEquipped = skinDatabases.skin[currentButtonIndex].index == selectedSkin;
        SetActionButton(isEquipped ? "Unequip" : "SELECT",
                        isEquipped ? Color.white : new Color(1f, 221f / 255f, 0f));
    }
    public void ResetItemOfPlayer()
    {
        DataManager.Ins.gameSave.idHat = "Hats_7";
        DataManager.Ins.gameSave.idShield = "Shield_2";
        DataManager.Ins.gameSave.idPant = "Pants_7";
        DataManager.Ins.SaveGame();
    }
    private void OnDisable()
    {
        if (hairManager.isSetHat || protectManager.isSetShield || pantsManager.isSetPant ||isResetClothes)
            SetSkinOfPlayer();

        if (!isResetClothes)
        {
            hairManager.SetHatOfPlayer();
            protectManager.SetShieldOfPlayer();
            pantsManager.SetPaintOfPlayer();
        }
    }
}
