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
    public WeaponDatabase weaponDB;

    public TextMeshProUGUI nameText;
    public Image image;
    public TextMeshProUGUI coin;
    public TextMeshProUGUI isLock;
    public TextMeshProUGUI damage;
    private int selectedOption = 0;

    public GameObject Weapon;

    public GameObject button;
    public bool isTouch = false;
    public Sprite gift;
    public Sprite gift_lock;

    public int indexGift;

    public TextMeshProUGUI coinOfPlayerText;
    public int coinOfPlayer;

    public GameObject MenuSelect;

    public GameObject[] buttons;
    public int indexWeapon;
    public RectTransform rawImageRect;

    //--------------Custom----------------
    [SerializeField] private GameObject material3;
    [SerializeField] private GameObject material2;

    [SerializeField] private GameObject imageWeaponss;
    [SerializeField] private GameObject listColor;
    [SerializeField] private GameObject textureImage;

    [SerializeField] private List<Mesh> listRawImage;
    [SerializeField] private GameObject weaponCustom;
    private void Start()
    {
        coinOfPlayer = PlayerPrefs.GetInt("coinMoney");
        UpdateWeapon(selectedOption);
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                indexWeapon = buttons[index].layer;
                PlayerPrefs.SetInt("MaterialOfWeapon" + selectedOption, indexWeapon);
                Weapon weapon = weaponDB.GetWeapon(selectedOption);
                //SetMaterial();
                SetButtonMaterial(buttons[index].layer);
                image.sprite = weapon.weaponImage[indexWeapon];
            });
        }
        //SetButtonWeapon();
    }
    private void Update()
    {
        coinOfPlayerText.text = coinOfPlayer.ToString();
        indexGift = PlayerPrefs.GetInt("Gift");
        if(selectedOption == 5 && indexGift != 1)
        {
            button.GetComponent<Button>().interactable = false;
        }
        else
        {
            button.GetComponent<Button>().interactable = true;
        }
        UpdateMenuCustom();
        CheckMaterial();
    }
    public void NextOption()
    {
        selectedOption++;
        if(selectedOption >= weaponDB.WeaponCount())
        {
            selectedOption = 0;
        }
        UpdateWeapon(selectedOption);
        PlayerPrefs.SetInt("MenuCustom" + selectedOption, 0);
        ChangeMeshWeapon();
        //SetButtonWeapon();
    }
    public void BackOption()
    {
        selectedOption--;
        if(selectedOption < 0)
        {
            selectedOption = weaponDB.WeaponCount() - 1;
        }
        UpdateWeapon(selectedOption);
        PlayerPrefs.SetInt("MenuCustom" + selectedOption, 0);
        ChangeMeshWeapon();
        //SetButtonWeapon();
    }
    public void UpdateWeapon(int selectedOption)
    {
        UpdateGift();
        int index = PlayerPrefs.GetInt("MaterialOfWeapon" + selectedOption, 0);
        Debug.Log(index);
        Weapon weapon = weaponDB.GetWeapon(selectedOption);

        if (index < 0 || index >= weapon.weaponImage.Count())
        {
            index = 0; // hoặc bạn có thể cho = weapon.weaponImage.Length - 1
            PlayerPrefs.SetInt("MaterialOfWeapon" + selectedOption, index);
        }

        image.sprite = weapon.weaponImage[index];
        nameText.text = weapon.weaponName;
        isLock.text = weapon.isLock;
        coin.text = weapon.coin;
        damage.text = weapon.damageWeapon;
        SetButtonWeapon();
        if (weapon.isBought || weapon.isGift)
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
        if (!weapon.isGift && selectedOption == 5)
        {
            image.sprite = gift_lock;
        }
    }
    public int GetSelectedOption()
    {
        return selectedOption;
    }
    public void BuyWeapon()
    {
        Weapon weapon = weaponDB.GetWeapon(selectedOption);
        if (weapon.isBought || weapon.isGift)
        {
            SaveWeapon();
            button.GetComponent<Image>().color = new Color(134f / 255f, 119f / 255f, 72f / 255f);
            weapon.isBought = true;
            coin.text = "Equipped";
            for (int i = 0; i < weaponDB.WeaponCount(); i++)
            {
                if (i == selectedOption)
                {
                    Weapon.GetComponent<MeshFilter>().mesh = weaponDB.weapon[i].meshWeapon;
                }
            }
        }
        else
        {
            if(selectedOption != 5)
            {
                if (int.Parse(weapon.coin) <= coinOfPlayer)
                {
                    SaveWeapon();
                    coinOfPlayer -= int.Parse(weapon.coin);
                    PlayerPrefs.SetInt("coinMoney", coinOfPlayer);
                    button.GetComponent<Image>().color = new Color(134f / 255f, 119f / 255f, 72f / 255f);
                    weapon.isBought = true;
                    coin.text = "Equipped";

                    for (int i = 0; i < weaponDB.WeaponCount(); i++)
                    {
                        if (i == selectedOption)
                        {
                            Weapon.GetComponent<MeshFilter>().mesh = weaponDB.weapon[i].meshWeapon;
                        }
                    }
                }
            }
        }
    }
    public void SetButtonWeapon()
    {
        int indexWeapon = PlayerPrefs.GetInt("IndexWeapon");
        Weapon weapon = weaponDB.GetWeapon(selectedOption);
        if (indexWeapon == selectedOption)
        {
            button.GetComponent<Image>().color = new Color(134f / 255f, 119f / 255f, 72f / 255f);
            coin.text = "Equipped";
            //SetButtonMaterial();
        }
        else
        {
            if (weapon.isBought || weapon.isGift)
            {
                button.GetComponent<Image>().color = new Color(254f / 255f, 204f / 255f, 45f / 255f);
                coin.text = "Select";
                //SetButtonMaterial();
            }
            else
            {
                button.GetComponent<Image>().color = new Color(68f / 255f, 224f / 255f, 22f / 255f);
            }
        }
    }
    public void SetWeapon(int x)
    {
        if (Weapon != null)
        {
            Weapon.GetComponent<MeshFilter>().mesh = weaponDB.weapon[x].meshWeapon;
            SetMaterial();
        }
    }
    public int LoadWeapon()
    {
        int x = PlayerPrefs.GetInt("IndexWeapon");
        return x;
    }
    private void UpdateGift()
    {
        if (indexGift == 1)
        {
            Weapon weapon1 = weaponDB.GetWeapon(5);
            //weapon1.weaponImage = gift;
            weapon1.weaponName = "Weapon";
            weapon1.isLock = "Unlock";
            weapon1.damageWeapon = "+10 damage";
            weapon1.coin = "Select";
            weapon1.isGift = true;
        }
        else
        {
            Weapon weapon1 = weaponDB.GetWeapon(5);
            //weapon1.weaponImage[0] = gift_lock;
            image.sprite = gift_lock;
            weapon1.weaponName = "Gift";
            weapon1.isLock = "Lock";
            weapon1.damageWeapon = "?";
            weapon1.coin = "Lock";
        }
    }
    public void SetMaterial()
    {
        Weapon weapon = weaponDB.GetWeapon(selectedOption);
        if (weapon.isBought || weapon.isGift)
        {
            int indexSelectOption = PlayerPrefs.GetInt("IndexWeapon");
            int indexMaterial = PlayerPrefs.GetInt("MaterialOfWeapon" + indexSelectOption);
            MeshRenderer meshRenderer = Weapon.GetComponent<MeshRenderer>();
            // Lấy toàn bộ materials ra
            Material[] mats = meshRenderer.materials;

            for (int j = 0; j < weaponDB.listOfMaterials[indexSelectOption].materialOfHammer[indexMaterial].materials.Length; j++)
            {
                mats[j] = weaponDB.listOfMaterials[indexSelectOption].materialOfHammer[indexMaterial].materials[j];
            }
            meshRenderer.materials = mats;
        }
    }
    public void SetButtonMaterial(int layer)   
    {
        int indexMaterial = PlayerPrefs.GetInt("ButtonOfMeterial" + selectedOption);

        if (indexMaterial == layer)
            {
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
        Weapon weapon = weaponDB.GetWeapon(selectedOption);
        if (weapon.isBought)
        {
            int indexMaterial = PlayerPrefs.GetInt("MaterialOfWeapon" + selectedOption);
            PlayerPrefs.SetInt("ButtonOfMeterial" + selectedOption, indexMaterial);
        }
    }
    public void SaveWeapon()
    {
        PlayerPrefs.SetInt("IndexWeapon", GetSelectedOption());
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
        }else if(selectedOption == 5)
        {
            material3.SetActive(false);
            material2.SetActive(true);
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
    public void ChangeMeshWeapon()
    {
        weaponCustom.GetComponent<MeshFilter>().mesh = listRawImage[selectedOption];
        if (selectedOption == 1)
        {
            rawImageRect.localPosition = new Vector3(26, -88, -0.001806259f);
            rawImageRect.sizeDelta = new Vector2(360, 500);
            //-0.001806259
        }
        if (selectedOption == 2)
        {
            rawImageRect.localPosition = new Vector3(34, -72, -0.001806259f);
            rawImageRect.sizeDelta = new Vector2(1100, 500);
            //-0.001806259
        }
        //-0.001806259
        if (selectedOption == 4)
        {
            rawImageRect.localPosition = new Vector3(-11, -79, -0.001806259f);
            rawImageRect.sizeDelta = new Vector2(360, 400);

        }
        if (selectedOption == 5)
        {
            rawImageRect.localPosition = new Vector3(33.28f, -72.7f, -0.001806259f);
            rawImageRect.sizeDelta = new Vector2(1200, 600);

        }
    }
}
