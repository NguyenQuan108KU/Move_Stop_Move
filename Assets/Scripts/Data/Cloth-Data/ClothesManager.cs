using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClothesManager : MonoBehaviour
{
    public ClothesSet[] clothesSet;
    public GameObject[] hairList;
    public int[] clothesPrices;
    public GameObject[] buttons;

    public int layerButton;

    public GameObject ButtonBuy;
    private TextMeshProUGUI txt;
    private int indexButton;

    [Header("Buy clothes")]
    public int coinOfPlayer;
    public TextMeshProUGUI coinOfClothes;

    public GameObject BuyByCoin;
    public GameObject BuyByAds;
    public GameObject SelectClothes;

    public GameObject initialShadingOfPlayer;
    public GameObject PantsOfPlayer;

    public Material materialDefaultOfPlayer;
    public HairManager hairManager;
    public ProtectManager protectManager;

    private void Start()
    {
        SetClothesPlayer();
        coinOfPlayer = PlayerPrefs.GetInt("coinMoney");
        txt = ButtonBuy.GetComponentInChildren<TextMeshProUGUI>();
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                layerButton = buttons[index].layer;
                SetClothesDefault(layerButton);
                SaveClothes();
                SetClothes(layerButton);
                if (coinOfClothes != null)
                {
                    coinOfClothes.text = clothesPrices[layerButton].ToString();
                }
            });
        }
    }
    private void Update()
    {
        indexButton = PlayerPrefs.GetInt("SlectClothes", -1);
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

    public void SetClothes(int x)
    {
        for (int i = 0; i < clothesSet.Count(); i++)
        {
            if (x == i)
            {
                if (hairManager != null)
                {
                    hairManager.ResetHair();
                }
                if (protectManager != null)
                {
                    protectManager.ResetProtect();
                }
                clothesSet[i].hatOfSet.SetActive(true);
                clothesSet[i].wingOfSet.SetActive(true);
                clothesSet[i].protectOfSet.SetActive(true);
                clothesSet[i].tailOfSet.SetActive(true);
                initialShadingOfPlayer.GetComponent<SkinnedMeshRenderer>().material = clothesSet[i].material;
                PantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = clothesSet[i].material;
            }
            else
            {
                clothesSet[i].hatOfSet.SetActive(false);
                clothesSet[i].wingOfSet.SetActive(false);
                clothesSet[i].protectOfSet.SetActive(false);
                clothesSet[i].tailOfSet.SetActive(false);
                //initialShadingOfPlayer.GetComponent<SkinnedMeshRenderer>().material = materialDefaultOfPlayer;
                //PantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = materialDefaultOfPlayer;
            }
        }
    }
    public void SaveClothes()
    {
        PlayerPrefs.SetInt("IndexClothes", layerButton);
    }
    public int LoadClothes()
    {
        int x = PlayerPrefs.GetInt("IndexClothes");
        return x;
    }
    public void ButtonClick()
    {
        if (txt.text == "SELECT")
        {
            PlayerPrefs.SetInt("SlectClothes", layerButton);
            txt.text = "Unequip";
            ButtonBuy.GetComponent<Image>().color = new Color(255f / 255f, 255f / 255f, 255f / 255f);
            SaveClothes();
            SetClothes(layerButton);
        }
        else if (txt.text == "Unequip")
        {
            PlayerPrefs.DeleteKey("SlectClothes");
            txt.text = "SELECT";
            ButtonBuy.GetComponent<Image>().color = new Color(255f / 255f, 221f / 255f, 0f / 255f);
            SetClothes(2);
        }
    }
    public void BuyClothes()
    {
        int indexBuy = PlayerPrefs.GetInt("IndexClothes");
        int price = clothesPrices[indexBuy];
        if (coinOfPlayer >= price)
        {
            coinOfPlayer -= price;
            PlayerPrefs.SetInt("coinMoney", coinOfPlayer);

            // Lưu trạng thái mua
            PlayerPrefs.SetInt("ClothesBought_" + indexBuy, 1);

            BuyByAds.SetActive(false);
            BuyByCoin.SetActive(false);
            SelectClothes.SetActive(true);
        }
    }
    public void SetClothesDefault(int i)
    {
        if (PlayerPrefs.GetInt("ClothesBought_" + i) == 1)
        {
            // Hair đã mua, hiển thị nút SELECT/UNEQUIP
            BuyByAds.SetActive(false);
            BuyByCoin.SetActive(false);
            SelectClothes.SetActive(true);
        }
        else
        {
            BuyByAds.SetActive(true);
            BuyByCoin.SetActive(true);
            SelectClothes.SetActive(false);
        }
    }
    public void SetClothesPlayer()
    {
        //int indexSet = PlayerPrefs.GetInt("SlectClothes");
        int index = PlayerPrefs.GetInt("SlectClothes", -1);
        if (index == -1)
        {
                clothesSet[2].hatOfSet.SetActive(false);
                clothesSet[2].wingOfSet.SetActive(false);
                clothesSet[2].protectOfSet.SetActive(false);
                clothesSet[2].tailOfSet.SetActive(false);
                initialShadingOfPlayer.GetComponent<SkinnedMeshRenderer>().material = materialDefaultOfPlayer;
                PantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = materialDefaultOfPlayer;
        }
        for (int i = 0; i < clothesSet.Count(); i++)
        {
            if (index == i)
            {
                clothesSet[i].hatOfSet.SetActive(true);
                clothesSet[i].wingOfSet.SetActive(true);
                clothesSet[i].protectOfSet.SetActive(true);
                clothesSet[i].tailOfSet.SetActive(true);
                initialShadingOfPlayer.GetComponent<SkinnedMeshRenderer>().material = clothesSet[index].material;
                PantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = clothesSet[index].material;
            }
            else
            {
                clothesSet[i].hatOfSet.SetActive(false);
                clothesSet[i].wingOfSet.SetActive(false);
                clothesSet[i].protectOfSet.SetActive(false);
                clothesSet[i].tailOfSet.SetActive(false);
                //initialShadingOfPlayer.GetComponent<SkinnedMeshRenderer>().material = materialDefaultOfPlayer;
                //PantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = materialDefaultOfPlayer;
            }
        }
    }
}
