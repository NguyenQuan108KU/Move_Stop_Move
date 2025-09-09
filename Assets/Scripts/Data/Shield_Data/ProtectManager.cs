using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProtectManager : MonoBehaviour
{
    public ShieldDatabases shieldsDatabases;
    public GameObject[] protectList;
    public int[] protectPrices;
    public GameObject shieldAnchor;

    public Button[] list_Buttons;
    public int currentButtonIndex;

    public GameObject ButtonBuy;
    public Image imageOfButtonSelect;
    public  TextMeshProUGUI textOfButtonSelect;

    [Header("Buy protect")]
    public int coinOfPlayer;
    public TextMeshProUGUI coinOfShield;

    public GameObject buyByCoin;
    public GameObject buyByAds;
    public GameObject selectShield;
    public bool isSetShield;
    public ClothesManager clothesManager;
    private void Start()
    {
        SetShieldOfPlayer();
        coinOfPlayer = PlayerPrefs.GetInt("coinMoney");
        for (int i = 0; i < list_Buttons.Length; i++)
        {
            int index = i;
            list_Buttons[index].onClick.AddListener(() =>
            {
                currentButtonIndex = index;
                StateShieldOfPlayer(currentButtonIndex);
                StateOfButton(currentButtonIndex);
                RefreshActionButton();
            });
        }
    }

    public void ButtonClick()
    {
        if (textOfButtonSelect.text == "SELECT")
        {
            if(clothesManager.isResetClothes)
                ResetSkinOfPlayer();
            clothesManager.isResetClothes = false;
            isSetShield = true;
            string shieldName = shieldsDatabases.shields[currentButtonIndex].index;
            DataManager.Ins.gameSave.idShield = shieldName;
            DataManager.Ins.SaveGame();
            SetActionButton("Unequip", Color.white);
        }
        else if (textOfButtonSelect.text == "Unequip")
        {
            clothesManager.isResetClothes = true;
            isSetShield = false;
            string shieldName = shieldsDatabases.shields[shieldsDatabases.shields.Length - 1].index;
            DataManager.Ins.gameSave.idShield = shieldName;
            DataManager.Ins.SaveGame();
            SetActionButton("SELECT", new Color(1f, 221f / 255f, 0f));
        }
        SetShieldOfPlayer();
    }
    public void BuyProtect()
    {
        string shieldName = shieldsDatabases.shields[currentButtonIndex].index;
        int coinOfShield = shieldsDatabases.shields[currentButtonIndex].coinOfshield;
        if (coinOfPlayer >= coinOfShield)
        {
            coinOfPlayer -= coinOfShield;
            PlayerPrefs.SetInt("coinMoney", coinOfPlayer);
            if (!DataManager.Ins.gameSave.objectsBought.Contains(shieldName))
            {
                DataManager.Ins.gameSave.objectsBought.Add(shieldName);
            }
            DataManager.Ins.SaveGame();
            buyByAds.SetActive(false);
            buyByCoin.SetActive(false);
            selectShield.SetActive(true);
        }
    }
    public void StateOfButton(int index)
    {
        string shieldName = shieldsDatabases.shields[index].index;
        if (DataManager.Ins.gameSave.objectsBought.Contains(shieldName))
        {  // Kiểm tra xem dữ liệu quần đã có quần vừa click chưa (Nếu có tức là đã mua thì set trạng thái select cho button)
            buyByAds.SetActive(false);
            buyByCoin.SetActive(false);
            selectShield.SetActive(true);
        }
        else
        {
            buyByAds.SetActive(true);
            buyByCoin.SetActive(true);
            selectShield.SetActive(false);
        }
    }
    public void StateShieldOfPlayer(int index)
    {
        if(clothesManager.isResetClothes)
            clothesManager.StateSkinOfPlayer(clothesManager.skinDatabases.skin.Length - 1);
        foreach (Transform child in shieldAnchor.transform)
        {
            Destroy(child.gameObject);
        }
        Instantiate(shieldsDatabases.shields[index].shieldPrefab, shieldAnchor.transform);
    }
    public void SetShieldOfPlayer()
    {
        if (shieldsDatabases == null || shieldAnchor == null) return;
        string shieldName = DataManager.Ins?.gameSave?.idShield;
        if (string.IsNullOrEmpty(shieldName)) return;
        //  Xóa mũ cũ (nếu có)
        foreach (Transform child in shieldAnchor.transform)
        {
            Destroy(child.gameObject);
        }

        //  Tìm mũ theo database và gắn vào transformHats
        for (int i = 0; i < shieldsDatabases.shields.Length; i++)
        {
            if (shieldsDatabases.shields[i].index == shieldName)
            {
                Instantiate(shieldsDatabases.shields[i].shieldPrefab, shieldAnchor.transform);
                break;
            }
        }
    }
    //public void SetProtectPlayer()
    //{
    //    int index = PlayerPrefs.GetInt("SlectProtect", -1);
    //    if (index == -1)
    //    {
    //        protectList[2].SetActive(true);
    //    }
    //    for (int i = 0; i < protectList.Count(); i++)
    //    {
    //        if (index == i)
    //        {
    //            protectList[i].SetActive(true);
    //        }
    //        else
    //        {
    //            protectList[i].SetActive(false);
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
        string selectedShield = DataManager.Ins.gameSave.idShield;
        if (string.IsNullOrEmpty(selectedShield)) return;

        bool isEquipped = shieldsDatabases.shields[currentButtonIndex].index == selectedShield;
        SetActionButton(isEquipped ? "Unequip" : "SELECT",
                        isEquipped ? Color.white : new Color(1f, 221f / 255f, 0f));
    }
    public void ResetSkinOfPlayer()
    {
        DataManager.Ins.gameSave.idSkin = "Skin_2";
        DataManager.Ins.SaveGame();
        clothesManager.SetSkinOfPlayer();
    }
    public void ResetProtectWhenSelect()
    {
        // Xoá trạng thái hair đã chọn
        PlayerPrefs.DeleteKey("SlectProtect");
        // Tắt toàn bộ hair
        for (int i = 0; i < protectList.Length; i++)
        {
            protectList[i].SetActive(false);
        }
        // Đưa UI về trạng thái ban đầu
        if (textOfButtonSelect != null)
        {
            textOfButtonSelect.text = "SELECT";
            ButtonBuy.GetComponent<Image>().color = new Color(1f, 221f / 255f, 0f);
        }
    }
    private void OnDisable()
    {
        if (!clothesManager.isResetClothes)
            SetShieldOfPlayer();
        else if(isSetShield || clothesManager.isResetClothes)
            clothesManager.SetSkinOfPlayer();
    }
}
