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
    public GameObject weaponOfPlayer;

    public GameObject button;
    public bool isTouch = false;

    public int indexGift;

    public TextMeshProUGUI coinOfPlayerText;
    public int coinOfPlayer;

    public GameObject MenuSelect;

    public GameObject[] buttons;
    public int indexWeapon;  //Vị trí của weapon


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
    public GameObject optionCustom;

    public List<GameObject> buttonOfMaterial;
    private int indexButtonOfMaterial;
    public Material[] listMaterialOfColor;

    public GameObject weaponPrefabs;
    public Transform weaponAnchor;

    public GameObject anchorWeapon;

    [Header("---------------------Weapon Ouline Select----------------------")]
    public GameObject weaponOutline;
    private GameObject currentOutline;
    public GameObject circleOptionTransform;
    public TextMeshProUGUI coinMain;
    private void Start()
    {
        coinOfPlayer = DataManager.Ins.gameSave.coin;
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                indexWeapon = index;
                PlayerPrefs.SetInt("MaterialOfWeapon" + selectedOption, indexWeapon);
                PlayerPrefs.Save();
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
                OnButtonClickedWeapon(buttons[index].transform);
            });
        }
        //for (int i = 0; i < buttonOfMaterial.Count; i++)
        //{
        //    indexButtonOfMaterial = 1;
        //    int x = i;
        //    buttonOfMaterial[x].GetComponent<Button>().onClick.AddListener(() =>
        //    {
        //        indexButtonOfMaterial = buttonOfMaterial[x].layer;
        //        Debug.Log(indexButtonOfMaterial);
        //    });
        //}
    }
    private void Update()
    {
        coinOfPlayerText.text = coinOfPlayer.ToString();
    }
    public void NextOption()
    {
        selectedOption++;
        if(selectedOption >= weaponData.WeaponCount())
        {
            selectedOption = 0;
        }
        UpdateWeapon(selectedOption);
        ChangeWeaponButtonColor(selectedOption);
        DestroyOutline(buttons[PlayerPrefs.GetInt("MaterialOfWeapon" + selectedOption)].transform);
        if (selectedOption == PlayerPrefs.GetInt("SelectOption"))
            OptionWeaponWhenStartGame();
        else
            LoadColorOfWeaponButton(selectedOption);
    }
    public void BackOption()
    {
        selectedOption--;
        if(selectedOption < 0)
        {
            selectedOption = weaponData.WeaponCount() - 1;
        }
        UpdateWeapon(selectedOption);
        ChangeWeaponButtonColor(selectedOption);
        DestroyOutline(buttons[PlayerPrefs.GetInt("MaterialOfWeapon" + selectedOption)].transform);
        if (selectedOption == PlayerPrefs.GetInt("SelectOption"))
            OptionWeaponWhenStartGame();
        else
            LoadColorOfWeaponButton(selectedOption);
    }
    public void UpdateWeapon(int selectedOptions)
    {
        int index = PlayerPrefs.GetInt("MaterialOfWeapon" + selectedOptions, 0);
        Weapon weapon = weaponData.GetWeapon(selectedOptions);
        
        foreach (Transform child in weaponAnchor.transform) {
            Destroy(child.gameObject);
        }
        GameObject newWeapon;
        if(PlayerPrefs.GetInt("SelectOption", 0) == selectedOption && weapon.isBought)
        {
            newWeapon = Instantiate(weapon.weaponPrefabs[index], weaponAnchor.transform);
            if ((PlayerPrefs.GetInt("IndexMaterialOfWeapon" + selectedOption, 1) == 0))
            {
                LoadColorOfWeapon(selectedOption);
                listColor.SetActive(true);
            }
            else
            {
                listColor.SetActive(false);
            }
        }
        else
        {
            newWeapon = Instantiate(weapon.weaponPrefabs[1], weaponAnchor.transform);
            listColor.SetActive(false);
        }
        if (!weapon.isBought)
        {
            listColor.SetActive(false);
        }
        // Gán vào weaponButtonColor để SetColorOfWeapon() dùng
        weaponCustom = newWeapon;
        //LoadColorOfWeapon(selectedOptions);

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
    public void SetWeaponStartGame()
    {
        if(DataManager.Ins.gameSave.idWeapon == "")
        {
            DataManager.Ins.gameSave.idWeapon = "Weapon_0";
            DataManager.Ins.SaveGame();
            foreach (Transform child in anchorWeapon.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newWeapon = Instantiate(weaponData.weapon[0].weaponOfPlayer, anchorWeapon.transform);
            Weapon = newWeapon.GetComponent<MeshFilter>();
            UpdateWeapon(0);
            ChangeWeaponButtonColor(selectedOption);
            foreach (Transform child in weaponAnchor.transform)
            {
                Destroy(child.gameObject);
            }
            OnButtonClickedWeapon(buttons[1].transform);
            button.GetComponent<Image>().color = new Color(134f / 255f, 119f / 255f, 72f / 255f);
            coin.text = "Equipped";
            SetMaterial();
        }
    }
    public void OptionWeaponWhenStartGame()
    {
        coinOfPlayer = DataManager.Ins.gameSave.coin;
        Debug.Log("coinOfPlayer - " + coinOfPlayer);
        coinMain.text = coinOfPlayer.ToString();
        int currentOption = PlayerPrefs.GetInt("SelectOption", 0);
        ChangeWeaponButtonColor(currentOption);
        int indexMaterialOfWeapon = PlayerPrefs.GetInt("IndexMaterialOfWeapon" + currentOption, 1);
        selectedOption = currentOption;
        Weapon weapon = weaponData.GetWeapon(currentOption);
        if(DataManager.Ins.gameSave.idWeapon != "")
        {
            UpdateWeapon(currentOption);
            foreach (Transform child in weaponAnchor.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newWeapon = Instantiate(weapon.weaponPrefabs[indexMaterialOfWeapon], weaponAnchor.transform);
            weaponCustom = newWeapon;
            if (indexMaterialOfWeapon == 0)
                LoadColorOfWeapon(currentOption);
            else
                LoadColorOfWeaponButton(currentOption);
            OnButtonClickedWeapon(buttons[indexMaterialOfWeapon].transform);
            SetButtonMaterial(indexMaterialOfWeapon);
        }
    }
    public void DisplayMenuSelectColor(int selectOptionOfWeapon)
    {
        Weapon weapon = weaponData.GetWeapon(selectOptionOfWeapon);
        if (weapon.isBought && (PlayerPrefs.GetInt("MaterialOfWeapon" + selectOptionOfWeapon) == 0))
        {
            listColor.SetActive(true);
        }
        else
        {
            listColor.SetActive(false);
        }
    }
    private void OnButtonClickedWeapon(Transform buttonTransform)
    {
        // Xoá viền cũ nếu có
        if (currentOutline != null)
        {
            Destroy(currentOutline);
        }
        // Sinh viền mới làm con của button
        currentOutline = Instantiate(weaponOutline, buttonTransform);
    }
    public void DestroyOutline(Transform buttonTransform)
    {
        if (currentOutline != null)
        {
            Destroy(currentOutline);
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
                    foreach (Transform child in anchorWeapon.transform)
                    {
                        Destroy(child.gameObject);
                    }
                    PlayerPrefs.SetInt("IndexMaterialOfWeapon" + selectedOption, indexWeapon);
                    PlayerPrefs.Save();
                    GameObject newWeapon =  Instantiate(weaponData.weapon[i].weaponOfPlayer, anchorWeapon.transform);
                    Weapon = newWeapon.GetComponent<MeshFilter>();
                }
            }
        }
        else
        {
            if (int.Parse(weapon.coin) <= coinOfPlayer && weaponData.weapon[selectedOption - 1].isBought){
                SaveWeapon();
                DataManager.Ins.gameSave.idWeapon = weaponData.GetWeapon(selectedOption).index;
                string weaponIndex = weapon.index;
                coinOfPlayer -= int.Parse(weapon.coin);
                DataManager.Ins.gameSave.coin = coinOfPlayer;
                DataManager.Ins.SaveGame();
                MenuSelect.SetActive(true);
                for (int i = 0; i < buttons.Length; i++)
                {
                    Image img = buttons[i].transform.GetChild(0).GetComponent<Image>();
                    img.sprite = weapon.weaponImage[i];
                }
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
                        foreach (Transform child in anchorWeapon.transform)
                        {
                            Destroy(child.gameObject);
                        }
                        SetButtonMaterial(1);
                        PlayerPrefs.SetInt("IndexMaterialOfWeapon" + selectedOption, 1);
                        PlayerPrefs.Save();
                        OnButtonClickedWeapon(buttons[1].transform);
                        GameObject newWeapon = Instantiate(weaponData.weapon[i].weaponOfPlayer, anchorWeapon.transform);
                        Weapon = newWeapon.GetComponent<MeshFilter>();
                    }
                    }
                }
        }
    }
    public void BuyWeaponByAds()
    {
        string weaponIndex = weaponData.weapon[2].index;
        if (!DataManager.Ins.gameSave.objectsBought.Contains(weaponIndex))
        {
            DataManager.Ins.gameSave.objectsBought.Add(weaponIndex);
        }
        DataManager.Ins.SaveGame();
        weaponData.weapon[2].isBought = true;
    }
    public void SetButtonWeapon()
    {
        Weapon weapon = weaponData.GetWeapon(selectedOption);
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
                foreach (Transform child in anchorWeapon.transform)
                {
                    Destroy(child.gameObject);
                }
                GameObject newWeapon = Instantiate(weaponData.weapon[i].weaponOfPlayer, anchorWeapon.transform);
                Weapon = newWeapon.GetComponent<MeshFilter>();
                SetMaterial();
            }
        }
    }
    public void SetMaterial()
    {
        int selectedOptions = PlayerPrefs.GetInt("SelectOption", 0);
        Weapon weapon = weaponData.GetWeapon(selectedOptions);
        if (weapon.isBought)
        {
            int indexSelectOption = PlayerPrefs.GetInt("SelectOption", 0);
            int indexMaterial = PlayerPrefs.GetInt("IndexMaterialOfWeapon" + indexSelectOption, 1);
            MeshRenderer meshRenderer = Weapon.GetComponent<MeshRenderer>();

            Material[] mats = meshRenderer.materials;
            for (int j = 0; j < weaponData.listOfMaterials[indexSelectOption].materialOfHammer[indexMaterial].materials.Length; j++)
            {
                mats[j] = weaponData.listOfMaterials[indexSelectOption].materialOfHammer[indexMaterial].materials[j];
                if(j == 2 && selectedOption == 0)
                {
                    mats[2] = weaponData.listOfMaterials[indexSelectOption].materialOfHammer[indexMaterial].materials[1];
                }
            }
            meshRenderer.materials = mats;
        }
    }
    public void SetButtonMaterial(int layer)
    {
        int indexMaterial = PlayerPrefs.GetInt("StateOfButton" + selectedOption, 1);
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
            int indexMaterial = PlayerPrefs.GetInt("MaterialOfWeapon" + selectedOption, 1);
            PlayerPrefs.SetInt("StateOfButton" + selectedOption, indexMaterial);
            PlayerPrefs.Save();
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
        }else if(selectedOption == 1)
        {
            material2.SetActive(true);
            material3.SetActive(true);
        }else if(selectedOption == 2)
        {
            material2.SetActive(false);
            material3.SetActive(false);
        }
    }
    public void SaveMenuCustom()
    {
        PlayerPrefs.SetInt("MenuCustom" + selectedOption, 1);
        PlayerPrefs.Save();
    }
    public void UpdateMenuCustom()
    {
        int index = PlayerPrefs.GetInt("MenuCustom" + selectedOption);
        if(index == 0)
        {
            imageWeaponss.SetActive(true);
        }
    }
    public void SetColorOfWeapon(int indexColor)
    {
        MeshRenderer renderer = weaponButtonColor.GetComponent<MeshRenderer>();
        MeshRenderer renderer1 = weaponCustom.GetComponent<MeshRenderer>();

        int targetCount = 1;
        if (selectedOption == 0) targetCount = 2;
        else if (selectedOption == 1) targetCount = 3;
        else if (selectedOption == 2) targetCount = 1;

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

            if (indexButtonOfMaterial - 1 < buttonOfMaterial.Count)
            {
                buttonOfMaterial[indexButtonOfMaterial - 1].GetComponent<Image>().color = newColor;
            }

            string key = "WeaponColor_" + selectedOption + "_" + indexButtonOfMaterial;
            PlayerPrefs.SetInt(key, indexColor);
            PlayerPrefs.Save();
        }

        renderer.materials = mats;
        renderer1.materials = mats1;
        for (int i = 0; i < mats.Length; i++)
        {
            weaponData.listOfMaterials[selectedOption].materialOfHammer[0].materials[i] = mats[i];
        }

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
        else if (selectedOption == 1) targetCount = 3;
        else if (selectedOption == 2) targetCount = 1;

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

                if (slot - 1 < buttonOfMaterial.Count)
                    buttonOfMaterial[slot - 1].GetComponent<Image>().color = loadedColor;
            }
        }

        renderer.materials = mats;
        renderer1.materials = mats1;
        for (int i = 0; i < mats.Length; i++)
        {
            weaponData.listOfMaterials[selectedOption].materialOfHammer[0].materials[i] = mats[i];
        }
        for (int i = 0; i < mats1.Length; i++)
        {
            weaponData.listOfMaterials[selectedOption].materialOfHammer[0].materials[i] = mats1[i];
        }
        SetMaterial();
    }
    public void LoadColorOfWeaponButton(int selectedOption)
    {
        MeshRenderer renderer = weaponButtonColor.GetComponent<MeshRenderer>();
        int targetCount = 1;
        if (selectedOption == 0) targetCount = 2;
        else if (selectedOption == 1) targetCount = 3;
        else if (selectedOption == 2) targetCount = 1;

        Material[] mats = Normalize(renderer.materials, targetCount);

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

                mats[slot - 1].color = loadedColor;

                if (slot - 1 < buttonOfMaterial.Count)
                    buttonOfMaterial[slot - 1].GetComponent<Image>().color = loadedColor;
            }
        }

        renderer.materials = mats;
        for (int i = 0; i < mats.Length; i++)
        {
            weaponData.listOfMaterials[selectedOption].materialOfHammer[0].materials[i] = mats[i];
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
    public void ChangeWeaponButtonColor(int select)
    {
        if (weaponButtonColor == null) return; // tránh lỗi khi object đã bị hủy

        if (select == 0)
        {
            foreach (Transform child in optionCustom.transform)
            {
                Destroy(child.gameObject);
            }
            buttonOfMaterial.Clear();
            for (int i = 0; i < weaponData.weapon[0].listOptionCustom.Count; i++)
            {
                GameObject newOption =  Instantiate(weaponData.weapon[0].listOptionCustom[i], optionCustom.transform);
                buttonOfMaterial.Add(newOption);
            }
            foreach (Transform child in rawImageRectOfButton.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newWeaponCustom = Instantiate(weaponData.weapon[0].weaponCustomer, rawImageRectOfButton.transform);
            weaponButtonColor = newWeaponCustom;
        }
        if (select == 1)
        {
            foreach (Transform child in optionCustom.transform)
            {
                Destroy(child.gameObject);
            }
            buttonOfMaterial.Clear();
            for (int i = 0; i < weaponData.weapon[1].listOptionCustom.Count; i++)
            {
                GameObject newOption = Instantiate(weaponData.weapon[1].listOptionCustom[i], optionCustom.transform);
                buttonOfMaterial.Add(newOption);
            }
            foreach (Transform child in rawImageRectOfButton.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newWeaponCustom = Instantiate(weaponData.weapon[1].weaponCustomer, rawImageRectOfButton.transform);
            weaponButtonColor = newWeaponCustom;
        }
        if (select == 2)
        {
            foreach (Transform child in optionCustom.transform)
            {
                Destroy(child.gameObject);
            }
            buttonOfMaterial.Clear();
            for (int i = 0; i < weaponData.weapon[2].listOptionCustom.Count; i++)
            {
                GameObject newOption = Instantiate(weaponData.weapon[2].listOptionCustom[i], optionCustom.transform);
                buttonOfMaterial.Add(newOption);
            }
            foreach (Transform child in rawImageRectOfButton.transform)
            {
                Destroy(child.gameObject);
            }
            GameObject newWeaponCustom = Instantiate(weaponData.weapon[2].weaponCustomer, rawImageRectOfButton.transform);
            weaponButtonColor = newWeaponCustom;
        }
        for (int i = 0; i < buttonOfMaterial.Count; i++)
        {
            indexButtonOfMaterial = 1;
            int x = i;
            foreach (Transform child in circleOptionTransform.transform)
            {
                Destroy(child.gameObject);
            }
            Instantiate(weaponData.weapon[select].circleOption[0], circleOptionTransform.transform);
            buttonOfMaterial[x].GetComponent<Button>().onClick.AddListener(() =>
            {
                indexButtonOfMaterial = buttonOfMaterial[x].layer;
                foreach (Transform child in circleOptionTransform.transform)
                {
                    Destroy(child.gameObject);
                }
                Instantiate(weaponData.weapon[select].circleOption[x], circleOptionTransform.transform);
            });
        }
    }

}
