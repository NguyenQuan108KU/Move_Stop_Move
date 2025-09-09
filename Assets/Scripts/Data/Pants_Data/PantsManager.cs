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
    public ClothesManager clothesManager;

    public bool isSetPant;
    [Header("-------------------Player Data-------------------")]
    public int coinOfPlayer;                //Tiền của người chơi 
    private void Start(){
        coinOfPlayer = DataManager.Ins.gameSave.coin;       //Lấy tiền từ dữ liệu 
        //Duyệt qua từng nút
        for (int i = 0; i < list_Buttons.Length; i++)
        {
            int index = i;
            list_Buttons[index].onClick.AddListener((UnityEngine.Events.UnityAction)(() =>
            {
                currentButtonIndex = index;                //Lưu lại index của từng nút button khi click 
                StatePaintOfPlayer(currentButtonIndex);      //Set trạng thái quần cho nhân vật khi click (set khi ấn vào các nút button quần)
                StateOfButton(currentButtonIndex);         //Trạng thái (mua/chưa mua) cho button
                RefreshActionButton();
            }));
        }
    }
    //public void StateOfPant(int x)
    //{
    //if (clothesManager != null)
    //{
    //  clothesManager.ResetClothes();
    //}
    //  if (pantsOfPlayer != null)
    //    pantsOfPlayer.material = pantsDatabases.pants[x].material;
    //}
    public void ButtonClick()
    {
        if (textOfButtonSelect.text == "SELECT")
        {
            if(clothesManager.isResetClothes)
                ResetSkinOfPlayer();
            clothesManager.isResetClothes = false;
            isSetPant = true;
            string pantName = pantsDatabases.pants[currentButtonIndex].index;
            DataManager.Ins.gameSave.idPant = pantName;
            DataManager.Ins.SaveGame();
            SetActionButton("Unequip", Color.white);
        }
        else if (textOfButtonSelect.text == "Unequip")
        {
            clothesManager.isResetClothes = true;
            isSetPant = false;
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
            PlayerPrefs.SetInt("coinMoney", coinOfPlayer);
            if (!DataManager.Ins.gameSave.objectsBought.Contains(pantName))
            {
                DataManager.Ins.gameSave.objectsBought.Add(pantName);
            }
            DataManager.Ins.SaveGame();
            buyByAds.SetActive(false);
            buyByCoin.SetActive(false);
            selectPaint.SetActive(true);
        }
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
        if (clothesManager.isResetClothes)
            clothesManager.StateSkinOfPlayer(clothesManager.skinDatabases.skin.Length - 1);
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
    //Hàm thay đổi text và màu của button khi chọn 
    private void SetActionButton(string text, Color color)
    {
        if (textOfButtonSelect != null)
            textOfButtonSelect.text = text;

        if (imageOfButtonSelect != null)
            imageOfButtonSelect.color = color;
    }
    private void RefreshActionButton()
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
    private void OnDisable()
    {
        if (!clothesManager.isResetClothes)
            SetPaintOfPlayer(); // quay lại đúng quần đã SELECT
        else if (isSetPant || clothesManager.isResetClothes)
            clothesManager.SetSkinOfPlayer();
    }
}
