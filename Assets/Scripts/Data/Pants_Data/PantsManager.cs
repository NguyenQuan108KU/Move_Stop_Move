using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class PantsManager : MonoBehaviour
{
    public PantsDatabases pantsDatabases;
    public GameObject[] buttons;
    public int[] paintsPrices;
    public GameObject pantsOfPlayer;
    public int layerButton;

    public GameObject ButtonBuy;
    private TextMeshProUGUI txt;
    private int indexButton;

    [Header("Buy Paints")]
    public int coinOfPlayer;
    public TextMeshProUGUI coinOfPaints;

    public GameObject BuyByCoin;
    public GameObject BuyByAds;
    public GameObject SelectPaint;

    public ClothesManager clothesManager;
    private void Start()
    {
        SetPaintsPlayer();
        coinOfPlayer = PlayerPrefs.GetInt("coinMoney");
        txt = ButtonBuy.GetComponentInChildren<TextMeshProUGUI>();
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; 
            buttons[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                layerButton = buttons[index].layer;
                SetPaintsDefault(layerButton);
                SavePants();
                SetPants(layerButton);
                if (coinOfPaints != null)
                {
                    coinOfPaints.text = paintsPrices[layerButton].ToString();
                }
            });
        }
    }
    private void Update()
    {
        indexButton = PlayerPrefs.GetInt("SlectPaint", -1);
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
    public void SetPants(int x)
    {
        if (clothesManager != null)
        {
            clothesManager.ResetClothes();
        }
        if (pantsOfPlayer != null)
            pantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = pantsDatabases.pants[x].material;
    }
    public void SavePants()
    {
        PlayerPrefs.SetInt("IndexPants",layerButton);

    }
    public int LoadPants()
    {
        int x = PlayerPrefs.GetInt("IndexPants");
        return x;
    }
    public void ButtonClick()
    {
        if (txt.text == "SELECT")
        {
            clothesManager.ResetClothesWhenSelect();
            clothesManager.isSetFull = false;
            PlayerPrefs.SetInt("SlectPaint", layerButton);
            txt.text = "Unequip";
            ButtonBuy.GetComponent<Image>().color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
            SavePants();
            SetPants(layerButton);
        }
        else if (txt.text == "Unequip")
        {
            PlayerPrefs.DeleteKey("SlectPaint");
            txt.text = "SELECT";
            ButtonBuy.GetComponent<Image>().color = new Color(255f / 255f, 221f / 255f, 0f / 255f);
            SetPants(6);
        }
    }
    public void BuyPaints()
    {
        int indexBuy = PlayerPrefs.GetInt("IndexPants");
        int price = paintsPrices[indexBuy];
        if (coinOfPlayer >= price)
        {
            coinOfPlayer -= price;
            PlayerPrefs.SetInt("coinMoney", coinOfPlayer);

            // Lưu trạng thái mua
            PlayerPrefs.SetInt("PantsBought_" + indexBuy, 1);

            BuyByAds.SetActive(false);
            BuyByCoin.SetActive(false);
            SelectPaint.SetActive(true);
        }
    }
    public void SetPaintsDefault(int i)
    {
        if (PlayerPrefs.GetInt("PantsBought_" + i) == 1)
        {
            BuyByAds.SetActive(false);
            BuyByCoin.SetActive(false);
            SelectPaint.SetActive(true);
        }
        else
        {
            BuyByAds.SetActive(true);
            BuyByCoin.SetActive(true);
            SelectPaint.SetActive(false);
        }
    }
    public void SetPaintsPlayer()
    {
        int index = PlayerPrefs.GetInt("SlectPaint", -1);
        if (index == -1)
        {
            if (clothesManager.isSetFull)
            {
                pantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = pantsDatabases.pants[7].material;
            }
            else
            {
                pantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = pantsDatabases.pants[6].material;
            }
        }
        else
        {
            pantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = pantsDatabases.pants[index].material;
        }
    }
    public void ResetPaints()
    {
        // Tắt toàn bộ pants
        pantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = pantsDatabases.pants[7].material;
    }
    public void ResetPaintsWhenSelect()
    {
        PlayerPrefs.DeleteKey("SlectPaint");
        pantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = pantsDatabases.pants[6].material;
        if(txt != null)
        {
            txt.text = "SELECT";
        }
        ButtonBuy.GetComponent<Image>().color = new Color(1f, 221f / 255f, 0f);
    }
}
