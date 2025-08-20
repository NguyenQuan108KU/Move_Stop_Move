using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HairManager : MonoBehaviour
{
    public GameObject[] hairList;
    public int[] hairPrices;
    public GameObject[] buttons;

    public int layerButton;

    public GameObject ButtonBuy;
    private TextMeshProUGUI txt;
    private int indexButton;

    [Header("Buy Hair")]
    public int coinOfPlayer;
    public TextMeshProUGUI coinOfHair;

    public GameObject BuyByCoin;
    public GameObject BuyByAds;
    public GameObject SelectHair;


    private void Start()
    {
        SetHairPlayer();
        coinOfPlayer = PlayerPrefs.GetInt("coinMoney");
        txt = ButtonBuy.GetComponentInChildren<TextMeshProUGUI>();
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                layerButton = buttons[index].layer;
                SetHairDefault(layerButton);
                SaveHat();
                SetHairs(layerButton);
                if(coinOfHair != null)
                {
                    coinOfHair.text = hairPrices[layerButton].ToString();
                }
            });
        }
    }
    private void Update()
    {
        indexButton = PlayerPrefs.GetInt("SlectHat", -1);
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

    public void SetHairs(int x)
    {
        for(int i = 0; i < hairList.Count(); i++)
        {
            if(x == i)
            {
                hairList[i].SetActive(true);
            }
            else
            {
                hairList[i].SetActive(false);
            }
        }
    }
    public void SaveHat()
    {
        PlayerPrefs.SetInt("IndexHat", layerButton);
    }
    public int LoadHats()
    {
        int x = PlayerPrefs.GetInt("IndexHat");
        return x;
    }
    public void ButtonClick()
    {
        if (txt.text == "SELECT")
        {
            PlayerPrefs.SetInt("SlectHat", layerButton);
            txt.text = "Unequip";
            ButtonBuy.GetComponent<Image>().color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
            SaveHat();
            SetHairs(layerButton);
        }
        else if (txt.text == "Unequip")
        {
            PlayerPrefs.DeleteKey("SlectHat");
            txt.text = "SELECT";
            ButtonBuy.GetComponent<Image>().color = new Color(255f / 255f, 221f / 255f, 0f / 255f);
            SetHairs(hairList.Count());
        }
    }
    public void BuyHair()
    {
        int indexBuy = PlayerPrefs.GetInt("IndexHat");
        int price = hairPrices[indexBuy];
        if (coinOfPlayer >= price)
        {
            coinOfPlayer -= price;
            PlayerPrefs.SetInt("coinMoney", coinOfPlayer);

            // Lưu trạng thái mua
            PlayerPrefs.SetInt("HairBought_" + indexBuy, 1);

            BuyByAds.SetActive(false);
            BuyByCoin.SetActive(false);
            SelectHair.SetActive(true);
        }
    }
    public void SetHairDefault(int i)
    {
            if (PlayerPrefs.GetInt("HairBought_" + i) == 1)
            {
                // Hair đã mua, hiển thị nút SELECT/UNEQUIP
                BuyByAds.SetActive(false);
                BuyByCoin.SetActive(false);
                SelectHair.SetActive(true);
            }
            else
            {
                BuyByAds.SetActive(true);
                BuyByCoin.SetActive(true);
                SelectHair.SetActive(false);
            }

    }
    public void SetHairPlayer()
    {
        Debug.Log(PlayerPrefs.GetInt("SlectHat", -1));
        int index = PlayerPrefs.GetInt("SlectHat", -1);
        if(index == -1)
        {
            hairList[6].SetActive(true);
        }
        for (int i = 0; i < hairList.Count(); i++)
        {
            if (index == i)
            {
                hairList[i].SetActive(true);
            }
            else
            {
                hairList[i].SetActive(false);
            }
        }
    }
}
