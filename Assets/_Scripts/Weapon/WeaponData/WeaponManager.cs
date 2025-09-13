using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    public WeaponDatabase weaponData;
    public TextMeshProUGUI nameText;
    public Image image;
    public TextMeshProUGUI coin;     //thông tin tiền của người chơi
    public TextMeshProUGUI isLock;   //cờ kiểm tra vũ khí đó được mở khở hay chưa
    public TextMeshProUGUI damage;   //thông tin về damage của vũ khí 
    private int selectedOption = 0;  //lựa chọn của vũ khí hiện tại

    public MeshFilter Weapon;

    public GameObject button;
    public bool isTouch = false;
    //public Sprite gift;
    //public Sprite gift_lock;

    public int indexGift;

    public TextMeshProUGUI coinOfPlayerText;
    public int coinOfPlayer;

    public GameObject MenuSelect;

    public GameObject[] buttons;
    public int indexWeapon;  //Vị trí của weapon
    public RectTransform rawImageRect;


    //--------------Custom----------------
    [SerializeField] private GameObject material3;
    [SerializeField] private GameObject material2;

    [SerializeField] private GameObject imageWeaponss;
    [SerializeField] private GameObject listColor;
    [SerializeField] private GameObject textureImage;

    [SerializeField] private List<Mesh> listRawImage;
    //[SerializeField] private GameObject weaponCustom;

    [SerializeField] private GameObject weaponButtonColor;
    [SerializeField] private GameObject weaponCustom;

    public RectTransform rawImageRectOfButton;
    public GameObject waeponButton;

    public GameObject[] buttonOfMaterial;
    private int indexButtonOfMaterial;
    public Material[] listMaterialOfColor;

    public GameObject weaponPrefabs;
    public Transform weaponAnchor;

    private void Start()
    {
        coinOfPlayer = PlayerPrefs.GetInt("coinMoney");
        UpdateWeapon(selectedOption);
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                indexWeapon = index;
                PlayerPrefs.SetInt("MaterialOfWeapon" + selectedOption, indexWeapon);
                Weapon weapon = weaponData.GetWeapon(selectedOption);
                SetButtonMaterial(index);
                foreach (Transform child in weaponAnchor.transform)
                {
                  Destroy(child.gameObject);
                }
                GameObject newWeapon = Instantiate(weapon.weaponPrefabs[indexWeapon], weaponAnchor.transform);
                weaponCustom = newWeapon;
                if(index == 0)
                    LoadColorOfWeapon(selectedOption);

                image.sprite = weapon.weaponImage[indexWeapon];
            });
        }
        for(int i = 0; i < buttonOfMaterial.Length; i++)
        {
            indexButtonOfMaterial = 1;
            int x = i;
            buttonOfMaterial[x].GetComponent<Button>().onClick.AddListener(() =>
            {
                indexButtonOfMaterial = buttonOfMaterial[x].layer;
            });
        }
    }
    private void Update()
    {
        coinOfPlayerText.text = coinOfPlayer.ToString();
        CheckMaterial();
    }
    public void NextOption()
    {
        selectedOption++;
        if(selectedOption >= weaponData.WeaponCount())
        {
            selectedOption = 0;
        }
        UpdateWeapon(selectedOption);
        ChangeWeaponButtonColor();
    }
    public void BackOption()
    {
        selectedOption--;
        if(selectedOption < 0)
        {
            selectedOption = weaponData.WeaponCount() - 1;
        }
        UpdateWeapon(selectedOption);
        ChangeWeaponButtonColor();
    }
    public void UpdateWeapon(int selectedOption)
    {
        int index = PlayerPrefs.GetInt("MaterialOfWeapon" + selectedOption, 0);
        Weapon weapon = weaponData.GetWeapon(selectedOption);
        foreach (Transform child in weaponAnchor.transform) {
            Destroy(child.gameObject);
        }
        GameObject newWeapon;
        newWeapon = Instantiate(weapon.weaponPrefabs[index], weaponAnchor.transform);
        // Gán vào weaponButtonColor để SetColorOfWeapon() dùng
        weaponCustom = newWeapon;
        LoadColorOfWeapon(selectedOption);

        nameText.text = weapon.weaponName;
        isLock.text = weapon.isLock;
        coin.text = weapon.coin;
        damage.text = weapon.damageWeapon;
        SetButtonWeapon();
        if (weapon.isBought)
        {
            MenuSelect.SetActive(true);
            for(int i = 0; i < buttons.Length; i++)
            {
                Image img = buttons[i].transform.GetChild(0).GetComponent<Image>();
                img.sprite = weapon.weaponImage[i];
            }
        }
        else
        {
            MenuSelect.SetActive(false);
        }
    }
    public void BuyWeapon()
    {
        Weapon weapon = weaponData.GetWeapon(selectedOption);
        if (weapon.isBought)
        {
            SaveWeapon();
            DataManager.Ins.gameSave.idWeapon = weapon.index;
            DataManager.Ins.SaveGame();

            button.GetComponent<Image>().color = new Color(134f / 255f, 119f / 255f, 72f / 255f);
            coin.text = "Equipped";
            for (int i = 0; i < weaponData.WeaponCount(); i++)
            {
                if (i == selectedOption)
                {
                    Weapon.mesh = weaponData.weapon[i].meshWeapon;
                }
            }
        }
        else
        {
            if (int.Parse(weapon.coin) <= coinOfPlayer){
                SaveWeapon();
                    DataManager.Ins.gameSave.idWeapon = weaponData.GetWeapon(selectedOption).index;
                    string weaponIndex = weapon.index;
                    coinOfPlayer -= int.Parse(weapon.coin);
                    PlayerPrefs.SetInt("coinMoney", coinOfPlayer);
                    button.GetComponent<Image>().color = new Color(134f / 255f, 119f / 255f, 72f / 255f);
                    weapon.isBought = true;
                    coin.text = "Equipped";
                    if (!DataManager.Ins.gameSave.objectsBought.Contains(weaponIndex))
                    {
                        DataManager.Ins.gameSave.objectsBought.Add(weaponIndex);
                    }
                    DataManager.Ins.SaveGame();
                for (int i = 0; i < weaponData.WeaponCount(); i++)
                    {
                        if (i == selectedOption)
                        {
                            Weapon.mesh = weaponData.weapon[i].meshWeapon;
                        }
                    }
                }
        }
    }
    public void SetButtonWeapon()
    {
        //int indexWeapon = PlayerPrefs.GetInt("IndexWeapon");
        Weapon weapon = weaponData.GetWeapon(selectedOption);
        //if (indexWeapon == selectedOption)
        if(DataManager.Ins.gameSave.idWeapon == weapon.index)
        {
            button.GetComponent<Image>().color = new Color(134f / 255f, 119f / 255f, 72f / 255f);
            coin.text = "Equipped";
        }
        else
        {
            if (weapon.isBought)
            {
                button.GetComponent<Image>().color = new Color(254f / 255f, 204f / 255f, 45f / 255f);
                coin.text = "Select";
            }
            else
            {
                button.GetComponent<Image>().color = new Color(68f / 255f, 224f / 255f, 22f / 255f);
            }
        }
    }
    public void SetWeapon()
    {
        for(int i = 0; i<  weaponData.WeaponCount(); i++)
        {
            if (weaponData.weapon[i].index == DataManager.Ins.gameSave.idWeapon)
            {
                Weapon.mesh = weaponData.weapon[i].meshWeapon;
                SetMaterial();
            }
        }
    }
    public void SetMaterial()
    {
        int selectedOptions = PlayerPrefs.GetInt("SelectOption");
        Weapon weapon = weaponData.GetWeapon(selectedOptions);
        if (weapon.isBought)
        {
            int indexSelectOption = PlayerPrefs.GetInt("SelectOption");
            int indexMaterial = PlayerPrefs.GetInt("MaterialOfWeapon" + indexSelectOption);
            MeshRenderer meshRenderer = Weapon.GetComponent<MeshRenderer>();

            Material[] mats = meshRenderer.materials;
            for (int j = 0; j < weaponData.listOfMaterials[indexSelectOption].materialOfHammer[indexMaterial].materials.Length; j++)
            {
                mats[j] = weaponData.listOfMaterials[indexSelectOption].materialOfHammer[indexMaterial].materials[j];
            }
            meshRenderer.materials = mats;
        }
    }
    public void SetButtonMaterial(int layer)
    {
        int indexMaterial = PlayerPrefs.GetInt("StateOfButton" + selectedOption);
        if (indexMaterial == layer && DataManager.Ins.gameSave.idWeapon == weaponData.weapon[selectedOption].index){
                button.GetComponent<Image>().color = new Color(134f / 255f, 119f / 255f, 72f / 255f);
                coin.text = "Equipped";
            }
            else
            {
                button.GetComponent<Image>().color = new Color(254f / 255f, 204f / 255f, 45f / 255f);
                coin.text = "Select";
            }
    }
    public void SaveButtonMaterial()
    {
        Weapon weapon = weaponData.GetWeapon(selectedOption);
        if (weapon.isBought)
        {
            int indexMaterial = PlayerPrefs.GetInt("MaterialOfWeapon" + selectedOption);
            PlayerPrefs.SetInt("StateOfButton" + selectedOption, indexMaterial);
        }
    }
    public void SaveWeapon()
    {
        PlayerPrefs.SetInt("SelectOption", selectedOption);  //Lưu select option
        PlayerPrefs.Save();
    }
    //----------------Custom Weapon-----------------------
    public void CheckMaterial()
    {
        if(selectedOption == 0)
        {
            material3.SetActive(false);
            material2.SetActive(true);
        }else if(selectedOption == 2)
        {
            material2.SetActive(true);
            material3.SetActive(true);
        }else if(selectedOption == 4)
        {
            material2.SetActive(false);
            material3.SetActive(false);
        }
    }
    public void SaveMenuCustom()
    {
        PlayerPrefs.SetInt("MenuCustom" + selectedOption, 1);
    }
    public void UpdateMenuCustom()
    {
        int index = PlayerPrefs.GetInt("MenuCustom" + selectedOption);
        if(index == 0)
        {
            imageWeaponss.SetActive(true);
            listColor.SetActive(false);
            textureImage.SetActive(false);
        }
    }
    //public void ChangeMeshWeapon()
    //{
    //    weaponCustom.GetComponent<MeshFilter>().mesh = listRawImage[selectedOption];
    //    if (selectedOption == 1)
    //    {
    //        rawImageRect.localPosition = new Vector3(26, -88, -0.001806259f);
    //        rawImageRect.sizeDelta = new Vector2(360, 500);
    //        //-0.001806259
    //    }
    //    if (selectedOption == 2)
    //    {
    //        rawImageRect.localPosition = new Vector3(34, -72, -0.001806259f);
    //        //rawImageRect.sizeDelta = new Vector2(1100, 500);
    //        //-0.001806259
    //    }
    //    //-0.001806259
    //    if (selectedOption == 4)
    //    {
    //        rawImageRect.localPosition = new Vector3(-11, -79, -0.001806259f);
    //        rawImageRect.sizeDelta = new Vector2(360, 400);

    //    }
    //    if (selectedOption == 5)
    //    {
    //        rawImageRect.localPosition = new Vector3(33.28f, -72.7f, -0.001806259f);
    //        rawImageRect.sizeDelta = new Vector2(1200, 600);

    //    }
    //}
    public void SetColorOfWeapon(int indexColor)
    {
        MeshRenderer renderer = weaponButtonColor.GetComponent<MeshRenderer>();
        MeshRenderer renderer1 = weaponCustom.GetComponent<MeshRenderer>();

        int targetCount = 1;
        if (selectedOption == 0) targetCount = 2;
        else if (selectedOption == 2) targetCount = 3;
        else if (selectedOption == 4) targetCount = 1;
        else targetCount = 3;

        Material[] mats = Normalize(renderer.materials, targetCount);
        Material[] mats1 = Normalize(renderer1.materials, targetCount);

        string[] colorString = new string[]
        {
        "0.043, 0, 1", "1, 0.741, 0", "1, 0, 0", "1, 0, 0.631",
        "0, 0, 0", "0, 1, 0.894", "1, 0.518, 0", "0.671, 1, 0",
        "0, 0.627, 1", "0.905, 0, 1"
        };

        if (indexButtonOfMaterial >= 1 && indexButtonOfMaterial <= targetCount)
        {
            float[] values = colorString[indexColor].Split(',').Select(s => float.Parse(s.Trim())).ToArray();
            Color newColor = new Color(values[0], values[1], values[2], 1f);

            mats[indexButtonOfMaterial - 1] = new Material(mats[indexButtonOfMaterial - 1]);
            mats1[indexButtonOfMaterial - 1] = new Material(mats1[indexButtonOfMaterial - 1]);

            mats[indexButtonOfMaterial - 1].color = newColor;
            mats1[indexButtonOfMaterial - 1].color = newColor;

            if (indexButtonOfMaterial - 1 < buttonOfMaterial.Length)
                buttonOfMaterial[indexButtonOfMaterial - 1].GetComponent<Image>().color = newColor;

            string key = "WeaponColor_" + selectedOption + "_" + indexButtonOfMaterial;
            PlayerPrefs.SetInt(key, indexColor);
            PlayerPrefs.Save();
        }

        renderer.materials = mats;
        renderer1.materials = mats1;

        for (int i = 0; i < mats1.Length; i++)
        {
            weaponData.listOfMaterials[selectedOption].materialOfHammer[0].materials[i] = mats1[i];
        }
        SetMaterial();
    }

    public void LoadColorOfWeapon(int selectedOption)
    {
        MeshRenderer renderer = weaponButtonColor.GetComponent<MeshRenderer>();
        MeshRenderer renderer1 = weaponCustom.GetComponent<MeshRenderer>();


        int targetCount = 1;
        if (selectedOption == 0) targetCount = 2;
        else if (selectedOption == 2) targetCount = 3;
        else if (selectedOption == 4) targetCount = 1;
        else targetCount = 3;

        Material[] mats = Normalize(renderer.materials, targetCount);
        Material[] mats1 = Normalize(renderer1.materials, targetCount);

        string[] colorString = new string[]
        {
        "0.043, 0, 1", "1, 0.741, 0", "1, 0, 0", "1, 0, 0.631",
        "0, 0, 0", "0, 1, 0.894", "1, 0.518, 0", "0.671, 1, 0",
        "0, 0.627, 1", "0.905, 0, 1"
        };

        for (int slot = 1; slot <= targetCount; slot++)
        {
            string key = "WeaponColor_" + selectedOption + "_" + slot;
            if (PlayerPrefs.HasKey(key))
            {
                int indexColor = PlayerPrefs.GetInt(key);
                float[] values = colorString[indexColor].Split(',').Select(s => float.Parse(s.Trim())).ToArray();
                Color loadedColor = new Color(values[0], values[1], values[2], 1f);

                mats[slot - 1] = new Material(mats[slot - 1]);
                mats1[slot - 1] = new Material(mats1[slot - 1]);

                mats[slot - 1].color = loadedColor;
                mats1[slot - 1].color = loadedColor;

                if (slot - 1 < buttonOfMaterial.Length)
                    buttonOfMaterial[slot - 1].GetComponent<Image>().color = loadedColor;
            }
        }

        renderer.materials = mats;
        renderer1.materials = mats1;

        for (int i = 0; i < mats1.Length; i++)
        {
            weaponData.listOfMaterials[selectedOption].materialOfHammer[0].materials[i] = mats1[i];
        }
        SetMaterial();
    }
    private Material[] Normalize(Material[] mats, int targetCount)
    {
        if (mats.Length > targetCount)
        {
            Array.Resize(ref mats, targetCount);
        }
        else if (mats.Length < targetCount && mats.Length > 0)
        {
            Material lastMat = mats[mats.Length - 1];
            Array.Resize(ref mats, targetCount);
            for (int i = mats.Length - 1; i >= 0; i--)
            {
                if (mats[i] == null) mats[i] = new Material(lastMat);
            }
        }
        return mats;
    }



    public void ChangeWeaponButtonColor()
    {

        if (weaponButtonColor == null) return; // tránh lỗi khi object đã bị hủy

        weaponButtonColor.GetComponent<MeshFilter>().mesh = listRawImage[selectedOption];
        if (selectedOption == 0)
        {
            rawImageRectOfButton.localPosition = new Vector3(-10.1f, -2.7f, -29.24f);
            rawImageRectOfButton.localRotation = Quaternion.Euler(0f, 11.9f, 72f);
            rawImageRectOfButton.sizeDelta = new Vector2(150, 150);
            waeponButton.transform.localScale = new Vector3(3500, 3500, 3500);
        }
        if (selectedOption == 2)
        {
            rawImageRectOfButton.localPosition = new Vector3(3.1f, -3.3f, -29.24f);
            rawImageRectOfButton.localRotation = Quaternion.Euler(0f, 11.9f, 72f);
            waeponButton.transform.localScale = new Vector3(9000, 3500, 3500);
        }
        if (selectedOption == 4)
        {
            rawImageRectOfButton.localPosition = new Vector3(-9f, 1f, -29.24f);
            waeponButton.transform.localScale = new Vector3(2750, 2750, 2750);
            rawImageRectOfButton.sizeDelta = new Vector2(150, 150f);
            rawImageRectOfButton.localRotation = Quaternion.Euler(0f, 11.9f, -12f);
        }

        if (selectedOption == 5)
        {
            rawImageRectOfButton.localPosition = new Vector3(1.2f, -1f, -29.24f);
            rawImageRectOfButton.localRotation = Quaternion.Euler(0f, 0, 82f);
            rawImageRectOfButton.sizeDelta = new Vector2(150, 550);
            waeponButton.transform.localScale = new Vector3(3500, 3500, 3500);
        }

    }

}
