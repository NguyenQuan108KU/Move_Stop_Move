using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        coinOfPlayer = PlayerPrefs.GetInt("coinMoney");
        UpdateWeapon(selectedOption);
        SetButtonWeapon();
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
    }
    public void NextOption()
    {
        selectedOption++;
        if(selectedOption >= weaponDB.WeaponCount())
        {
            selectedOption = 0;
        }
        UpdateWeapon(selectedOption);
        SetButtonWeapon();
    }
    public void BackOption()
    {
        selectedOption--;
        if(selectedOption < 0)
        {
            selectedOption = weaponDB.WeaponCount() - 1;
        }
        UpdateWeapon(selectedOption);
        SetButtonWeapon();
    }
    public void UpdateWeapon(int selectedOption)
    {
        if (indexGift == 1)
        {
            Weapon weapon1 = weaponDB.GetWeapon(5);
            weapon1.weaponImage = gift;
            weapon1.weaponName = "Weapon";
            weapon1.isLock = "Unlock";
            weapon1.damageWeapon = "+10 damage";
        }
        else
        {
            Weapon weapon1 = weaponDB.GetWeapon(5);
            weapon1.weaponImage = gift_lock;
            weapon1.weaponName = "Gift";
            weapon1.isLock = "Lock";
            weapon1.damageWeapon = "?";
        }

        Weapon weapon = weaponDB.GetWeapon(selectedOption);
        image.sprite = weapon.weaponImage;
        nameText.text = weapon.weaponName;
        isLock.text = weapon.isLock;
        damage.text = weapon.damageWeapon;
        SetButtonWeapon();
    }
    public int GetSelectedOption()
    {
        return selectedOption;
    }
    public void BuyWeapon()
    {
        //Weapon weapon = weaponDB.GetWeapon(selectedOption);
        button.GetComponent<Image>().color = new Color(134f / 255f, 119f / 255f, 72f / 255f);
        coin.text = "UnEquip";

        for (int i = 0; i < weaponDB.WeaponCount(); i++)
        {
            if (i == selectedOption)
            {
                Weapon.GetComponent<MeshFilter>().mesh = weaponDB.weapon[i].meshWeapon;
            }
        }
    }
    public void SetButtonWeapon()
    {
        int indexWeapon = PlayerPrefs.GetInt("IndexWeapon");
        if(indexWeapon == selectedOption)
        {
            button.GetComponent<Image>().color = new Color(134f / 255f, 119f / 255f, 72f / 255f);
            coin.text = "UnEquip";
        }
        else
        {
            Weapon weapon = weaponDB.GetWeapon(selectedOption);
            button.GetComponent<Image>().color = new Color(254f / 255f, 204f / 255f, 45f / 255f);
            coin.text = "Equip";
        }
    }
    public void SetWeapon(int x)
    {
        if (Weapon != null)
            Weapon.GetComponent<MeshFilter>().mesh = weaponDB.weapon[x].meshWeapon;
    }
    public int LoadWeapon()
    {
        int x = PlayerPrefs.GetInt("IndexWeapon");
        return x;
    }
    public void BuyWeapons()
    {
        Weapon weapon = weaponDB.GetWeapon(selectedOption);
        if(int.Parse(weapon.coin) <= coinOfPlayer)
        {

        }
    }
}
