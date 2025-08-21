using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProtectManager : MonoBehaviour
{
    public GameObject[] protectList;
    public int[] protectPrices;
    public GameObject[] buttons;

    public int layerButton;

    public GameObject ButtonBuy;
    private TextMeshProUGUI txt;
    private int indexButton;

    [Header("Buy protect")]
    public int coinOfPlayer;
    public TextMeshProUGUI coinOfProtect;

    public GameObject BuyByCoin;
    public GameObject BuyByAds;
    public GameObject SelectProtect;


    private void Start()
    {
        SetProtectPlayer();
        coinOfPlayer = PlayerPrefs.GetInt("coinMoney");
        txt = ButtonBuy.GetComponentInChildren<TextMeshProUGUI>();
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                layerButton = buttons[index].layer;
                SetProtectDefault(layerButton);
                SaveProtect();
                SetProtect(layerButton);
                if (coinOfProtect != null)
                {
                    coinOfProtect.text = protectPrices[layerButton].ToString();
                }
            });
        }
    }
    private void Update()
    {
        indexButton = PlayerPrefs.GetInt("SlectProtect", -1);
        if (layerButton == indexButton)
        {
            txt.text = "Unequip";
            ButtonBuy.GetComponent<Image>().color = new Color(1f, 1f, 1f);
        }
        else
        {
            txt.text = "SELECT";
            ButtonBuy.GetComponent<Image>().color = new Color(1f, 221f / 255f, 0f);
        }
    }

    public void SetProtect(int x)
    {
        for (int i = 0; i < protectList.Count(); i++)
        {
            if (x == i)
            {
                protectList[i].SetActive(true);
            }
            else
            {
                protectList[i].SetActive(false);
            }
        }
    }
    public void SaveProtect()
    {
        PlayerPrefs.SetInt("IndexProtect", layerButton);
    }
    public int LoadProtect()
    {
        int x = PlayerPrefs.GetInt("IndexProtect");
        return x;
    }
    public void ButtonClick()
    {
        if (txt.text == "SELECT")
        {
            PlayerPrefs.SetInt("SlectProtect", layerButton);
            txt.text = "Unequip";
            ButtonBuy.GetComponent<Image>().color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
            SaveProtect();
            SetProtect(layerButton);
        }
        else if (txt.text == "Unequip")
        {
            PlayerPrefs.DeleteKey("SlectProtect");
            txt.text = "SELECT";
            ButtonBuy.GetComponent<Image>().color = new Color(255f / 255f, 221f / 255f, 0f / 255f);
            SetProtect(protectList.Count());
        }
    }
    public void BuyProtect()
    {
        int indexBuy = PlayerPrefs.GetInt("IndexProtect");
        int price = protectPrices[indexBuy];
        if (coinOfPlayer >= price)
        {
            coinOfPlayer -= price;
            PlayerPrefs.SetInt("coinMoney", coinOfPlayer);

            // Lưu trạng thái mua
            PlayerPrefs.SetInt("ProtectBought_" + indexBuy, 1);

            BuyByAds.SetActive(false);
            BuyByCoin.SetActive(false);
            SelectProtect.SetActive(true);
        }
    }
    public void SetProtectDefault(int i)
    {
        if (PlayerPrefs.GetInt("ProtectBought_" + i) == 1)
        {
            // Hair đã mua, hiển thị nút SELECT/UNEQUIP
            BuyByAds.SetActive(false);
            BuyByCoin.SetActive(false);
            SelectProtect.SetActive(true);
        }
        else
        {
            BuyByAds.SetActive(true);
            BuyByCoin.SetActive(true);
            SelectProtect.SetActive(false);
        }

    }
    public void SetProtectPlayer()
    {
        int index = PlayerPrefs.GetInt("SlectProtect", -1);
        if (index == -1)
        {
            protectList[2].SetActive(true);
        }
        for (int i = 0; i < protectList.Count(); i++)
        {
            if (index == i)
            {
                protectList[i].SetActive(true);
            }
            else
            {
                protectList[i].SetActive(false);
            }
        }
    }
    public void ResetProtect()
    {
        // Xoá trạng thái hair đã chọn
        PlayerPrefs.DeleteKey("SlectProtect");

        // Tắt toàn bộ hair
        for (int i = 0; i < protectList.Length; i++)
        {
            protectList[i].SetActive(false);
        }
        // Đưa UI về trạng thái ban đầu
        txt.text = "SELECT";
        ButtonBuy.GetComponent<Image>().color = new Color(1f, 221f / 255f, 0f);
    }
}
