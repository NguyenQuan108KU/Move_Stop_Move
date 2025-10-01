using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class PantsManager : MonoBehaviour
{
    [Header("-------------------Pants Data-------------------")]
    public PantsDatabases pantsDatabases;         //Dữ liệu quần trong game
    public int[] paintsPrices;                      
    public SkinnedMeshRenderer pantsOfPlayer;    //GameObject quần nhân vật
    [Header("-------------------UI Elements-------------------")]
    public Button[] list_Buttons;       //Danh sách các nút chọn quần  
    public int currentButtonIndex;      //Dùng để lưu trữ vị trí các nút chọn quần 
    public GameObject selectPaint;      //Nút chọn quần 
    public GameObject buyByCoin;                //Nút mua quần bằng tiền 
    public GameObject buyByAds;                 //Nút mua quần bằng xem quảng cáo  
    public TextMeshProUGUI textOfButtonSelect;  //Chữ của nút button khi chọn (Chưa chọn thì text là Select khi đã chọn thì là Unequip"
    public Image imageOfButtonSelect;           //Màu của nút button khi chọn
    public TextMeshProUGUI coinOfPaints;
    public GameObject optionPant;
    public ClothesManager clothesManager;
    public HairManager hairManager;
    public ShieldManager shieldManager;

    public GameObject borderPrefab; // prefab khung viền (Image)
    private GameObject currentBorder; // viền đang hiển thị

    public GameObject equippedPrefab;
    private GameObject currentEquipped;
    public GameObject lockOfHat;
    public TextMeshProUGUI coinOfPant;
    public TextMeshProUGUI textOfPlayer;
    public ApplyFullSetOfPlayer playerInfo;
    public bool isSetPant;
    [Header("-------------------Player Data-------------------")]
    public int coinOfPlayer;                //Tiền của người chơi 
    private void Start(){
        EquippedClothes(list_Buttons[PlayerPrefs.GetInt("IndexChoosePant")].transform);
        StateOfButton(PlayerPrefs.GetInt("IndexChoosePant"));
        coinOfPlayer = DataManager.Ins.gameSave.coin;
        textOfPlayer.text = coinOfPlayer.ToString();
        //Duyệt qua từng nút
        for (int i = 0; i < list_Buttons.Length; i++)
        {
            int index = i;
            list_Buttons[index].onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                coinOfPant.text = pantsDatabases.pants[index].coinOfPant.ToString();
                currentButtonIndex = index;                //Lưu lại index của từng nút button khi click 
                StatePaintOfPlayer(currentButtonIndex);      //Set trạng thái quần cho nhân vật khi click (set khi ấn vào các nút button quần)
                StateOfButton(currentButtonIndex);         //Trạng thái (mua/chưa mua) cho button
                RefreshActionButton();
                OnButtonClicked(list_Buttons[index].transform);
            }));
        }
    }
    public void SetLock()
    {
        for (int i = 0; i < list_Buttons.Length; i++)
        {
            string pantName = pantsDatabases.pants[i].index;
            Transform buttonTransform = list_Buttons[i].transform;
            Transform oldLock = buttonTransform.Find("LockIcon");
            if (oldLock != null)
            {
                Destroy(oldLock.gameObject);
            }
            if (!DataManager.Ins.gameSave.objectsBought.Contains(pantName))
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
        if (isSetPant)
            currentEquipped = Instantiate(equippedPrefab, buttonTransform);
    }
    public void ButtonClick()
    {
        isSetPant = true;
        clothesManager.isSetClothes = false;
        hairManager.isSetHat = false;
        shieldManager.isSetShield = false;
        if (textOfButtonSelect.text == "SELECT")
        {
            //if(clothesManager.isResetClothes)
            //    ResetSkinOfPlayer();
            //clothesManager.isResetClothes = false;
            //isSetPant = true;
            EquippedClothes(list_Buttons[currentButtonIndex].transform);
            PlayerPrefs.SetInt("IndexChoosePant", currentButtonIndex);
            string pantName = pantsDatabases.pants[currentButtonIndex].index;
            DataManager.Ins.gameSave.idPant = pantName;
            DataManager.Ins.SaveGame();
            SetActionButton("Unequip", Color.white);
            //Reset mũ của nhân vật
            hairManager.ResetDataOfHat();
            hairManager.RefreshActionButton();
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
            isSetPant = false;
            EquippedClothes(list_Buttons[currentButtonIndex].transform);
            string pantName = pantsDatabases.pants[pantsDatabases.pants.Length - 1].index;
            DataManager.Ins.gameSave.idPant = pantName;
            DataManager.Ins.SaveGame();
            SetActionButton("SELECT", new Color(1f, 221f / 255f, 0f));
        }
        SetPaintOfPlayer();
    }
    public void BuyPants()
    {
        string pantName = pantsDatabases.pants[currentButtonIndex].index;
        int coinOfPant = pantsDatabases.pants[currentButtonIndex].coinOfPant;
        if(coinOfPlayer >= coinOfPant)
        {
            coinOfPlayer -= coinOfPant;
            DataManager.Ins.gameSave.coin = coinOfPlayer;
            DataManager.Ins.SaveGame();
            textOfPlayer.text = coinOfPlayer.ToString();
            playerInfo.SetCoinPlayer();
            if (!DataManager.Ins.gameSave.objectsBought.Contains(pantName))
            {
                DataManager.Ins.gameSave.objectsBought.Add(pantName);
            }
            DataManager.Ins.SaveGame();
            SetLock();
            buyByAds.SetActive(false);
            buyByCoin.SetActive(false);
            selectPaint.SetActive(true);
        }
    }
    public void BuyPantByAds()
    {
        string pantName = pantsDatabases.pants[currentButtonIndex].index;
        if (!DataManager.Ins.gameSave.objectsBought.Contains(pantName))
        {
            DataManager.Ins.gameSave.objectsBought.Add(pantName);
        }
        DataManager.Ins.SaveGame();
        SetLock();
        buyByAds.SetActive(false);
        buyByCoin.SetActive(false);
        selectPaint.SetActive(true);
    }

    //Set trạng thái của button nếu đã mua quần thì sẽ chỉ hiển thị nút để select nếu chưa mua thì hiển thị 2 nút mua (mua bằng tiền hoặc xem quảng cáo)
    public void StateOfButton(int index){
        string pantName = pantsDatabases.pants[index].index;             //Lấy index của quần khi click vào button quần 
        if (DataManager.Ins.gameSave.objectsBought.Contains(pantName)){  // Kiểm tra xem dữ liệu quần đã có quần vừa click chưa (Nếu có tức là đã mua thì set trạng thái select cho button)
            buyByAds.SetActive(false);
            buyByCoin.SetActive(false);
            selectPaint.SetActive(true);
        }
        else{
            buyByAds.SetActive(true);
            buyByCoin.SetActive(true);
            selectPaint.SetActive(false);
        }
    }
   public void StatePaintOfPlayer(int index)
    {
        //if (clothesManager.isResetClothes)
        //    clothesManager.StateSkinOfPlayer(clothesManager.skinDatabases.skin.Length - 1);
        pantsOfPlayer.material = pantsDatabases.pants[index].material;
    }
    public void SetPaintOfPlayer()
    {
        if (pantsDatabases == null || pantsOfPlayer == null) return;
        string pantName = DataManager.Ins?.gameSave?.idPant;
        if (string.IsNullOrEmpty(pantName)) return;
        for (int i = 0; i < pantsDatabases.pants.Length; i++)
        {
            if (pantsDatabases.pants[i].index == pantName)
            {
                pantsOfPlayer.material = pantsDatabases.pants[i].material;
            }
        }
    }
    public void ResetPant()
    {
        pantsOfPlayer.material = pantsDatabases.pants[7].material;
    }
    public void ResetDataOfPant()
    {
        string pantName = pantsDatabases.pants[pantsDatabases.pants.Length - 1].index;
        DataManager.Ins.gameSave.idPant = pantName;
        DataManager.Ins.SaveGame();
    }
    //Hàm thay đổi text và màu của button khi chọn 
    private void SetActionButton(string text, Color color)
    {
        if (textOfButtonSelect != null)
            textOfButtonSelect.text = text;

        if (imageOfButtonSelect != null)
            imageOfButtonSelect.color = color;
    }
    public void RefreshActionButton()
    {
        string selectedPant = DataManager.Ins.gameSave.idPant;
        if (string.IsNullOrEmpty(selectedPant)) return;

        bool isEquipped = pantsDatabases.pants[currentButtonIndex].index == selectedPant;
        SetActionButton(isEquipped ? "Unequip" : "SELECT",
                        isEquipped ? Color.white : new Color(1f, 221f / 255f, 0f));
    }
    public void ResetSkinOfPlayer()
    {
        DataManager.Ins.gameSave.idSkin = "Skin_2";
        DataManager.Ins.SaveGame();
        clothesManager.SetSkinOfPlayer();
    }
    public void ResetPaintsWhenSelect()
{
    PlayerPrefs.DeleteKey("SlectPaint");
    pantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = pantsDatabases.pants[6].material;
    if (textOfButtonSelect != null)
    {
        textOfButtonSelect.text = "SELECT";
    }
    selectPaint.GetComponent<Image>().color = new Color(1f, 221f / 255f, 0f);
}
    public void CheckSetPant()
    {
        if (isSetPant)
            SetPaintOfPlayer();
    }
    public void DisplayOptionPant()
    {
        SetLock();
        isSetPant = DataManager.Ins.gameSave.isSetPant;
        if (isSetPant)
        {
            optionPant.SetActive(true);
            OnButtonClicked(list_Buttons[PlayerPrefs.GetInt("IndexChoosePant")].transform);
        }
        else
        {
            optionPant.SetActive(false);
            OnButtonClicked(list_Buttons[0].transform);
        }
    }
    public void DisplaySelectOption()
    {
        if (isSetPant)
        {
            OnButtonClicked(list_Buttons[PlayerPrefs.GetInt("IndexChoosePant")].transform);
        }
        else
        {
            OnButtonClicked(list_Buttons[0].transform);
            StatePaintOfPlayer(0);
            StateOfButton(0);
            EquippedClothes(list_Buttons[currentButtonIndex].transform);
        }
    }
    private void OnDisable()
    {
        ResetPant();
        DataManager.Ins.gameSave.isSetHat = hairManager.isSetHat;
        DataManager.Ins.gameSave.isSetClothes = clothesManager.isSetClothes;
        DataManager.Ins.gameSave.isSetShield = shieldManager.isSetShield;
        DataManager.Ins.gameSave.isSetPant = isSetPant;
        DataManager.Ins.SaveGame();
    }
}
