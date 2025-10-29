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

    public GameObject optionClothes;
    public HairManager hairManager;
    public ShieldManager shieldManager;
    public PantsManager pantsManager;
    public bool isSetClothes;

    public GameObject borderPrefab; // prefab khung viền (Image)
    private GameObject currentBorder; // viền đang hiển thị\

    public GameObject equippedPrefab;
    private GameObject currentEquipped;
    public GameObject lockOfHat;
    public TextMeshProUGUI coinOfSkin;
    public TextMeshProUGUI textOfPlayer;
    public ApplyFullSetOfPlayer playerInfo;

    private void Start()
    {
        EquippedClothes(list_Buttons[PlayerPrefs.GetInt("IndexChooseClothes")].transform);
        StateOfButton(PlayerPrefs.GetInt("IndexChooseClothes"));
        coinOfPlayer = DataManager.Ins.gameSave.coin;
        textOfPlayer.text = coinOfPlayer.ToString();
        for (int i = 0; i < list_Buttons.Length; i++)
        {
            int index = i;
            list_Buttons[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                coinOfSkin.text = skinDatabases.skin[index].coinOfSkin.ToString();
                currentButtonIndex = index;
                StateSkinOfPlayer(currentButtonIndex);
                StateOfButton(currentButtonIndex);
                RefreshActionButton();
                OnButtonClicked(list_Buttons[index].transform);
            });
        }
    }
    public void SetLock()
    {
        for (int i = 0; i < list_Buttons.Length; i++)
        {
            string skinName = skinDatabases.skin[i].index;
            Transform buttonTransform = list_Buttons[i].transform;
            Transform oldLock = buttonTransform.Find("LockIcon");
            if (oldLock != null)
            {
                Destroy(oldLock.gameObject);
            }
            if (!DataManager.Ins.gameSave.objectsBought.Contains(skinName))
            {
                GameObject newLock = Instantiate(lockOfHat, buttonTransform);
                newLock.name = "LockIcon"; // đặt tên để dễ tìm & xóa
            }
        }
    }
    private void OnButtonClicked(Transform buttonTransform)
    {
        // Xoá viền cũ nếu có
        if (currentBorder != null)
        {
            Destroy(currentBorder);
        }

        // Sinh viền mới làm con của button
        currentBorder = Instantiate(borderPrefab, buttonTransform);
    }
    public void EquippedClothes(Transform buttonTransform)
    {
        if (currentEquipped != null)
        {
            Destroy(currentEquipped);
        }

        // Sinh viền mới làm con của button
        if(isSetClothes)
            currentEquipped = Instantiate(equippedPrefab, buttonTransform);
    }
    public void ButtonClick()
    {
        hairManager.isSetHat = false;
        shieldManager.isSetShield = false;
        pantsManager.isSetPant = false;
        isSetClothes = true;
        if (textOfButtonSelect.text == "SELECT")
        {
            //isResetClothes = true;
            //ResetItemOfPlayer();
            //hairManager.isSetHat = false;
            //shieldManager.isSetShield = false;
            //pantsManager.isSetPant = false;
            EquippedClothes(list_Buttons[currentButtonIndex].transform);
            string skinName = skinDatabases.skin[currentButtonIndex].index;
            DataManager.Ins.gameSave.idSkin = skinName;
            DataManager.Ins.SaveGame();
            SetActionButton("Unequip", Color.white);
            PlayerPrefs.SetInt("IndexChooseClothes", currentButtonIndex);
            //Reset quần
            pantsManager.ResetDataOfPant();
            pantsManager.RefreshActionButton();
            //Reset khiên
            shieldManager.ResetDataOdShield();
            shieldManager.RefreshActionButton();
            //Reset mũ của nhân vật
            hairManager.ResetDataOfHat();
            hairManager.RefreshActionButton();
        }
        else if (textOfButtonSelect.text == "Unequip")
        {
            isSetClothes = false;
            EquippedClothes(list_Buttons[currentButtonIndex].transform);
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
            DataManager.Ins.gameSave.coin = coinOfPlayer;
            DataManager.Ins.SaveGame();
            textOfPlayer.text = coinOfPlayer.ToString();
            playerInfo.SetCoinPlayer();
            if (!DataManager.Ins.gameSave.objectsBought.Contains(skinName))
            {
                DataManager.Ins.gameSave.objectsBought.Add(skinName);
            }
            SetLock();
            DataManager.Ins.gameSave.idSkin = skinName;
            DataManager.Ins.SaveGame();
            buyByAds.SetActive(false);
            buyByCoin.SetActive(false);
            selectPaint.SetActive(true);

            hairManager.isSetHat = false;
            shieldManager.isSetShield = false;
            pantsManager.isSetPant = false;
            isSetClothes = true;

            EquippedClothes(list_Buttons[currentButtonIndex].transform);
            SetActionButton("Unequip", Color.white);
            PlayerPrefs.SetInt("IndexChooseClothes", currentButtonIndex);
            //Reset quần
            pantsManager.ResetDataOfPant();
            pantsManager.RefreshActionButton();
            //Reset khiên
            shieldManager.ResetDataOdShield();
            shieldManager.RefreshActionButton();
            //Reset mũ của nhân vật
            hairManager.ResetDataOfHat();
            hairManager.RefreshActionButton();
        }
    }
    public void BuyClothesByAds()
    {
        string skinName = skinDatabases.skin[currentButtonIndex].index;
        if (!DataManager.Ins.gameSave.objectsBought.Contains(skinName))
        {
            DataManager.Ins.gameSave.objectsBought.Add(skinName);
        }
        SetLock();
        DataManager.Ins.SaveGame();
        buyByAds.SetActive(false);
        buyByCoin.SetActive(false);
        selectPaint.SetActive(true);
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
    public void ResetClothes()
    {
        foreach (Transform anchor in list_anchorsOfSkin)
        {
            foreach (Transform child in anchor)
            {
                Destroy(child.gameObject);
            }
        }
        initialShadingOfPlayer.material = skinDatabases.skin[2].materialOfPlayer;
        pantsOfPlayer.material = skinDatabases.skin[2].materialOfPlayer;
    }
    public void ResetDatOfClothes()
    {
        string skinName = skinDatabases.skin[skinDatabases.skin.Length - 1].index;
        DataManager.Ins.gameSave.idSkin = skinName;
        DataManager.Ins.SaveGame();
    }
    private void SetActionButton(string text, Color color)
    {
        if (textOfButtonSelect != null)
            textOfButtonSelect.text = text;

        if (imageOfButtonSelect != null)
            imageOfButtonSelect.color = color;
    }
    public void RefreshActionButton()
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
    public void CheckSetSkin()
    {
        if (isSetClothes)
            SetSkinOfPlayer();
    }
    public void DisplayOptionClothes()
    {
        SetLock();
        isSetClothes = DataManager.Ins.gameSave.isSetClothes;
        if (isSetClothes)
        {
            optionClothes.SetActive(true);
            OnButtonClicked(list_Buttons[PlayerPrefs.GetInt("IndexChooseClothes")].transform);
            RefreshActionButton();
        }
        else
        {
            optionClothes.SetActive(false);
            OnButtonClicked(list_Buttons[0].transform);
        }
    }
    public void DisplaySelectOption()
    {
        if (isSetClothes)
        {
            OnButtonClicked(list_Buttons[PlayerPrefs.GetInt("IndexChooseClothes")].transform);
            RefreshActionButton();
        }
        else
        {
            OnButtonClicked(list_Buttons[0].transform);
            StateSkinOfPlayer(0);
            StateOfButton(0);
            EquippedClothes(list_Buttons[currentButtonIndex].transform);
            RefreshActionButton();
        }
    }
    private void OnDisable()
    {
        ResetClothes();
        DataManager.Ins.gameSave.isSetHat = hairManager.isSetHat;
        DataManager.Ins.gameSave.isSetClothes = isSetClothes;
        DataManager.Ins.gameSave.isSetShield = shieldManager.isSetShield;
        DataManager.Ins.gameSave.isSetPant = pantsManager.isSetPant;
        DataManager.Ins.SaveGame();
    }
}
