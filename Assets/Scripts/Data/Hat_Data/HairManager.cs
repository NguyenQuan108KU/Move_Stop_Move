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

    public ClothesManager clothesManager;
    private void Start()
    {
        coinOfPlayer = PlayerPrefs.GetInt("coinMoney");
        for (int i = 0; i < list_Buttons.Length; i++)
        {
            int index = i;
            list_Buttons[index].onClick.AddListener(() =>
            {
                currentButtonIndex = index;
                StateHatOfPlayer(currentButtonIndex);
                StateOfButton(currentButtonIndex);
                RefreshActionButton();
            });
        }
    }
    //private void Update()
    //{
    //    indexButton = PlayerPrefs.GetInt("SlectHat", -1);
    //    if (currentButtonIndex == indexButton)
    //    {
    //        textOfButtonSelect.text = "Unequip";
    //        imageOfButtonSelect.color = new Color(1f, 1f, 1f);
    //    }
    //    else
    //    {
    //        textOfButtonSelect.text = "SELECT";
    //        imageOfButtonSelect.color = new Color(1f, 221f / 255f, 0f);
    //    }
    //}

    //public void SetHairs(int x)
    //{
    //    if(clothesManager != null)
    //    {
    //        clothesManager.ResetClothes();
    //    }
    //    for(int i = 0; i < hairList.Count(); i++)
    //    {
    //        if(x == i)
    //        {
    //            hairList[i].SetActive(true);
    //        }
    //        else
    //        {
    //            hairList[i].SetActive(false);
    //        }
    //    }
    //}
    //public int LoadHats()
    //{
    //    int x = PlayerPrefs.GetInt("IndexHat");
    //    return x;
    //}
    public void ButtonClick()
    {
        if (textOfButtonSelect.text == "SELECT")
        {
            if (clothesManager.isResetClothes)
                ResetSkinOfPlayer();
            clothesManager.isResetClothes = false;
            isSetHat = true;
            string hatName = hatsDatabases.hats[currentButtonIndex].index;
            DataManager.Ins.gameSave.idHat = hatName;
            DataManager.Ins.SaveGame();
            SetActionButton("Unequip", Color.white);
        }
        else if (textOfButtonSelect.text == "Unequip")
        {
            clothesManager.isResetClothes = true;
            isSetHat = false;
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
            PlayerPrefs.SetInt("coinMoney", coinOfPlayer);
            if (!DataManager.Ins.gameSave.objectsBought.Contains(hatName))
            {
                DataManager.Ins.gameSave.objectsBought.Add(hatName);
            }
            DataManager.Ins.SaveGame();
            buyByAds.SetActive(false);
            buyByCoin.SetActive(false);
            selectPaint.SetActive(true);
        }
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
        if (clothesManager.isResetClothes)
            clothesManager.StateSkinOfPlayer(clothesManager.skinDatabases.skin.Length - 1);
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

    //public void SetHairPlayer()
    //{
    //    int index = PlayerPrefs.GetInt("SlectHat", -1);
    //    if(index == -1)
    //    {
    //        hairList[6].SetActive(true);
    //    }
    //    for (int i = 0; i < hairList.Count(); i++)
    //    {
    //        if (index == i)
    //        {
    //            hairList[i].SetActive(true);
    //        }
    //        else
    //        {
    //            hairList[i].SetActive(false);
    //        }
    //    }
    //}
    private void SetActionButton(string text, Color color)
    {
        if (textOfButtonSelect != null)
            textOfButtonSelect.text = text;

        if (imageOfButtonSelect != null)
            imageOfButtonSelect.color = color;
    }
    private void RefreshActionButton()
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
    private void OnDisable()
    {
        if (!clothesManager.isResetClothes)
            SetHatOfPlayer();
        else if(isSetHat || clothesManager.isResetClothes)
            clothesManager.SetSkinOfPlayer();
    }
}
