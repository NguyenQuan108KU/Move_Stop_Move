using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShieldManager : MonoBehaviour
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
    public GameObject optionShield;
    public ClothesManager clothesManager;
    public HairManager hairManager;
    public PantsManager pantsManager;

    public GameObject borderPrefab; // prefab khung viền (Image)
    private GameObject currentBorder; // viền đang hiển thị

    public GameObject equippedPrefab;
    private GameObject currentEquipped;
    public GameObject lockOfHat;

    public TextMeshProUGUI textOfPlayer;
    public ApplyFullSetOfPlayer playerInfo;
    private void Start()
    {
        EquippedClothes(list_Buttons[PlayerPrefs.GetInt("IndexChooseShield")].transform);
        StateOfButton(PlayerPrefs.GetInt("IndexChooseShield"));
        coinOfPlayer = DataManager.Ins.gameSave.coin;
        textOfPlayer.text = coinOfPlayer.ToString();
        //SetShieldOfPlayer();
        coinOfPlayer = PlayerPrefs.GetInt("coinMoney");
        for (int i = 0; i < list_Buttons.Length; i++)
        {
            int index = i;
            list_Buttons[index].onClick.AddListener(() =>
            {
                coinOfShield.text = shieldsDatabases.shields[index].coinOfshield.ToString();
                currentButtonIndex = index;
                StateShieldOfPlayer(currentButtonIndex);
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
            string shieldName = shieldsDatabases.shields[i].index;
            Transform buttonTransform = list_Buttons[i].transform;
            Transform oldLock = buttonTransform.Find("LockIcon");
            if (oldLock != null)
            {
                Destroy(oldLock.gameObject);
            }
            if (!DataManager.Ins.gameSave.objectsBought.Contains(shieldName))
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
        if (isSetShield)
            currentEquipped = Instantiate(equippedPrefab, buttonTransform);
    }
    public void ButtonClick()
    {
        if (textOfButtonSelect.text == "SELECT")
        {
        isSetShield = true;
        clothesManager.isSetClothes = false;
        hairManager.isSetHat = false;
        pantsManager.isSetPant = false;
            //if(clothesManager.isResetClothes)
            //    ResetSkinOfPlayer();
            EquippedClothes(list_Buttons[currentButtonIndex].transform);
            PlayerPrefs.SetInt("IndexChooseShield", currentButtonIndex);
            string shieldName = shieldsDatabases.shields[currentButtonIndex].index;
            DataManager.Ins.gameSave.idShield = shieldName;
            DataManager.Ins.SaveGame();
            SetActionButton("Unequip", Color.white);
            //Reset mũ
            pantsManager.ResetDataOfPant();
            pantsManager.RefreshActionButton();
            //Reset mũ của nhân vật
            hairManager.ResetDataOfHat();
            hairManager.RefreshActionButton();
            //Reset clothes
            clothesManager.ResetDatOfClothes();
            clothesManager.RefreshActionButton();

        }
        else if (textOfButtonSelect.text == "Unequip")
        {
            isSetShield = false;
            EquippedClothes(list_Buttons[currentButtonIndex].transform);
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
            DataManager.Ins.gameSave.coin = coinOfPlayer;
            DataManager.Ins.SaveGame();
            textOfPlayer.text = coinOfPlayer.ToString();
            playerInfo.SetCoinPlayer();
            if (!DataManager.Ins.gameSave.objectsBought.Contains(shieldName))
            {
                DataManager.Ins.gameSave.objectsBought.Add(shieldName);
            }
            SetLock();
            DataManager.Ins.SaveGame();
            buyByAds.SetActive(false);
            buyByCoin.SetActive(false);
            selectShield.SetActive(true);
        }
    }
    public void BuyProtectByAds()
    {
        string shieldName = shieldsDatabases.shields[currentButtonIndex].index;
        if (!DataManager.Ins.gameSave.objectsBought.Contains(shieldName))
        {
            DataManager.Ins.gameSave.objectsBought.Add(shieldName);
        }
        SetLock();
        DataManager.Ins.SaveGame();
        buyByAds.SetActive(false);
        buyByCoin.SetActive(false);
        selectShield.SetActive(true);
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
        //if(clothesManager.isResetClothes)
        //    clothesManager.StateSkinOfPlayer(clothesManager.skinDatabases.skin.Length - 1);
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
            }
        }
    }
    public void ResetShield()
    {
        foreach (Transform child in shieldAnchor.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void ResetDataOdShield()
    {
        string shieldName = shieldsDatabases.shields[shieldsDatabases.shields.Length - 1].index;
        DataManager.Ins.gameSave.idShield = shieldName;
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
    public void CheckSetShield()
    {
        if (isSetShield)
            SetShieldOfPlayer();
    }
    public void DisplayOptionShield()
    {
        SetLock();
        isSetShield = DataManager.Ins.gameSave.isSetShield;
        if (isSetShield)
        {
            optionShield.SetActive(true);
            OnButtonClicked(list_Buttons[PlayerPrefs.GetInt("IndexChooseShield")].transform);
        }
        else
        {
            optionShield.SetActive(false);
            OnButtonClicked(list_Buttons[0].transform);
        }
    }
    public void DisplaySelectOption()
    {
        if (isSetShield)
        {
            OnButtonClicked(list_Buttons[PlayerPrefs.GetInt("IndexChooseShield")].transform);
        }
        else
        {
            OnButtonClicked(list_Buttons[0].transform);
            StateShieldOfPlayer(0);
            StateOfButton(0);
            EquippedClothes(list_Buttons[currentButtonIndex].transform);
        }
    }
    private void OnDisable()
    {
        ResetShield();
        DataManager.Ins.gameSave.isSetHat = hairManager.isSetHat;
        DataManager.Ins.gameSave.isSetClothes = clothesManager.isSetClothes;
        DataManager.Ins.gameSave.isSetShield = isSetShield;
        DataManager.Ins.gameSave.isSetPant = pantsManager.isSetPant;
        DataManager.Ins.SaveGame();
    }
}
