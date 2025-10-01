using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HairManager : MonoBehaviour
{
    [Header("-------------------Hats Data-------------------")]
    public HatDatabases hatsDatabases;
    public GameObject[] hairList;
    public int[] hairPrices;
    public GameObject hatAnchor;

    public Button[] list_Buttons;
    public int currentButtonIndex;

    public GameObject ButtonBuy;
    public Image imageOfButtonSelect;
    public TextMeshProUGUI textOfButtonSelect;

    [Header("Buy Hair")]
    public int coinOfPlayer;
    public TextMeshProUGUI coinOfHair;

    public GameObject buyByCoin;
    public GameObject buyByAds;
    public GameObject selectPaint;

    public bool isSetHat;
    public GameObject optionHat;
    public ClothesManager clothesManager;
    public ShieldManager shieldManager;
    public PantsManager pantsManager;
    public GameObject lockOfHat;

    public GameObject borderPrefab; // prefab khung viền (Image)
    private GameObject currentBorder; // viền đang hiển thị

    public GameObject equippedPrefab;
    private GameObject currentEquipped;
    public TextMeshProUGUI coinOfHat;
    public TextMeshProUGUI textOfPlayer;
    public ApplyFullSetOfPlayer playerInfo;

    private void Start()
    {
        if (!isSetHat && !pantsManager.isSetPant && !shieldManager.isSetShield && !clothesManager.isSetClothes)
        {
            optionHat.SetActive(true);
        }
        EquippedClothes(list_Buttons[PlayerPrefs.GetInt("IndexChooseHat")].transform);
        StateOfButton(PlayerPrefs.GetInt("IndexChooseHat"));
        coinOfPlayer = DataManager.Ins.gameSave.coin;
        textOfPlayer.text = coinOfPlayer.ToString();
        for (int i = 0; i < list_Buttons.Length; i++)
        {
            int index = i;
            list_Buttons[index].onClick.AddListener(() =>
            {
                coinOfHat.text = hatsDatabases.hats[index].coinOfHat.ToString();
                currentButtonIndex = index;
                StateHatOfPlayer(currentButtonIndex);
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
            string hatName = hatsDatabases.hats[i].index;
            Transform buttonTransform = list_Buttons[i].transform;
            Transform oldLock = buttonTransform.Find("LockIcon");
            if (oldLock != null)
            {
                Destroy(oldLock.gameObject);
            }
            if (!DataManager.Ins.gameSave.objectsBought.Contains(hatName))
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
        if (isSetHat)
            currentEquipped = Instantiate(equippedPrefab, buttonTransform);
    }
    public void ButtonClick()
    {
        if (textOfButtonSelect.text == "SELECT")
        {
            isSetHat = true;
            clothesManager.isSetClothes = false;
            pantsManager.isSetPant = false;
            shieldManager.isSetShield = false;
            EquippedClothes(list_Buttons[currentButtonIndex].transform);
            PlayerPrefs.SetInt("IndexChooseHat", currentButtonIndex);
            string hatName = hatsDatabases.hats[currentButtonIndex].index;
            DataManager.Ins.gameSave.idHat = hatName;
            DataManager.Ins.SaveGame();
            SetActionButton("Unequip", Color.white);
            //Reset quần
            pantsManager.ResetDataOfPant();
            pantsManager.RefreshActionButton();
            //Reset khiên
            shieldManager.ResetDataOdShield();
            shieldManager.RefreshActionButton();
            //Reset clothes
            clothesManager.ResetDatOfClothes();
            clothesManager.RefreshActionButton();
        }
        else if (textOfButtonSelect.text == "Unequip")
        {
            //clothesManager.isResetClothes = true;
            isSetHat = false;
            EquippedClothes(list_Buttons[currentButtonIndex].transform);
            string hatName = hatsDatabases.hats[hatsDatabases.hats.Length - 1].index;
            DataManager.Ins.gameSave.idHat = hatName;
            DataManager.Ins.SaveGame();
            SetActionButton("SELECT", new Color(1f, 221f / 255f, 0f));
        }
        SetHatOfPlayer();
    }
    public void BuyHair()
    {
        
        string hatName = hatsDatabases.hats[currentButtonIndex].index;
        int coinOfHat = hatsDatabases.hats[currentButtonIndex].coinOfHat;
        if (coinOfPlayer >= coinOfHat)
        {
            coinOfPlayer -= coinOfHat;
            DataManager.Ins.gameSave.coin = coinOfPlayer;
            DataManager.Ins.SaveGame();
            textOfPlayer.text = coinOfPlayer.ToString();
            playerInfo.SetCoinPlayer();
            if (!DataManager.Ins.gameSave.objectsBought.Contains(hatName))
            {
                DataManager.Ins.gameSave.objectsBought.Add(hatName);
            }
            SetLock();
            DataManager.Ins.SaveGame();
            buyByAds.SetActive(false);
            buyByCoin.SetActive(false);
            selectPaint.SetActive(true);
        }
    }
    public void BuyHairByAds()
    {
        string hatName = hatsDatabases.hats[currentButtonIndex].index;
        if (!DataManager.Ins.gameSave.objectsBought.Contains(hatName))
        {
            DataManager.Ins.gameSave.objectsBought.Add(hatName);
        }
        SetLock();
        DataManager.Ins.SaveGame();
        buyByAds.SetActive(false);
        buyByCoin.SetActive(false);
        selectPaint.SetActive(true);
    }
    public void StateOfButton(int index)
    {
        string hatName = hatsDatabases.hats[index].index;             //Lấy index của quần khi click vào button quần 
        if (DataManager.Ins.gameSave.objectsBought.Contains(hatName))
        {  // Kiểm tra xem dữ liệu quần đã có quần vừa click chưa (Nếu có tức là đã mua thì set trạng thái select cho button)
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
    public void StateHatOfPlayer(int index)
    {
        //if (clothesManager.isResetClothes)
        //    clothesManager.StateSkinOfPlayer(clothesManager.skinDatabases.skin.Length - 1);
        foreach (Transform child in hatAnchor.transform)
        {
            Destroy(child.gameObject);
        }
        Instantiate(hatsDatabases.hats[index].hatPrefab, hatAnchor.transform);
    }
    public void SetHatOfPlayer()
    {
        if (hatsDatabases == null || hatAnchor == null) return;
        string hatName = DataManager.Ins?.gameSave?.idHat;
        if (string.IsNullOrEmpty(hatName)) return;

        //  Xóa mũ cũ (nếu có)
        foreach (Transform child in hatAnchor.transform)
        {
            Destroy(child.gameObject);
        }

        //  Tìm mũ theo database và gắn vào transformHats
        for (int i = 0; i < hatsDatabases.hats.Length; i++)
        {
            if (hatsDatabases.hats[i].index == hatName)
            {
                Instantiate(hatsDatabases.hats[i].hatPrefab, hatAnchor.transform);
            }
        }
    }
    public void ResetHat()
    {
        foreach (Transform child in hatAnchor.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void ResetDataOfHat()
    {
        string hatName = hatsDatabases.hats[hatsDatabases.hats.Length - 1].index;
        DataManager.Ins.gameSave.idHat = hatName;
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
        string selectedHat = DataManager.Ins.gameSave.idHat;
        if (string.IsNullOrEmpty(selectedHat)) return;
        bool isEquipped = hatsDatabases.hats[currentButtonIndex].index == selectedHat;
        SetActionButton(isEquipped ? "Unequip" : "SELECT",
                        isEquipped ? Color.white : new Color(1f, 221f / 255f, 0f));
    }
    public void ResetSkinOfPlayer()
    {
        DataManager.Ins.gameSave.idSkin = "Skin_2";
        DataManager.Ins.SaveGame();
        clothesManager.SetSkinOfPlayer();
    }
    public void CheckSetHat()
    {
        if (isSetHat)
            SetHatOfPlayer();
    }
    public void DisplayOptionHat()
    {
        SetLock();
        isSetHat = DataManager.Ins.gameSave.isSetHat;
        if (isSetHat)
        {
            optionHat.SetActive(true);
            OnButtonClicked(list_Buttons[PlayerPrefs.GetInt("IndexChooseHat")].transform);
            RefreshActionButton();
        }
        else
        {
            optionHat.SetActive(false);
            OnButtonClicked(list_Buttons[0].transform);
        }
        if (!isSetHat && !pantsManager.isSetPant && !shieldManager.isSetShield && !clothesManager.isSetClothes)
        {
            optionHat.SetActive(true);
        }
    }
    public void DisplaySelectOption()
    {
        if (isSetHat)
        {
            OnButtonClicked(list_Buttons[PlayerPrefs.GetInt("IndexChooseHat")].transform);
        }
        else
        {
            OnButtonClicked(list_Buttons[0].transform);
            StateHatOfPlayer(0);
            StateOfButton(0);
            EquippedClothes(list_Buttons[currentButtonIndex].transform);
        }
    }
    private void OnDisable()
    {
        ResetHat();
        DataManager.Ins.gameSave.isSetHat = isSetHat;
        DataManager.Ins.gameSave.isSetClothes = clothesManager.isSetClothes;
        DataManager.Ins.gameSave.isSetShield = shieldManager.isSetShield;
        DataManager.Ins.gameSave.isSetPant = pantsManager.isSetPant;
        DataManager.Ins.SaveGame();
    }
}
