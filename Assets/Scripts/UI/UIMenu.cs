using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public int score;
    public TextMeshProUGUI scoreMenu;
    public PantsManager pants;
    public HairManager hair;
    public WeaponManager weapon;
    public ClothesManager clothes;
    public ProtectManager protect;

    public TextMeshProUGUI coinOfPlayerText;
    void Start()
    {
        Time.timeScale = 1.0f;
        score = PlayerPrefs.GetInt("coinMoney");
        //pants.SetPants(pants.LoadPants());
        pants.SetPaintsPlayer();
        hair.SetHairPlayer();
        weapon.SetWeapon(weapon.LoadWeapon());
        clothes.SetClothesPlayer();
        protect.SetProtectPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        coinOfPlayerText.text = score.ToString();
        scoreMenu.text = score.ToString();
    }
    public void SaveWeapon()
    {
        //PlayerPrefs.SetInt("IndexWeapon", WeaponManager.instance.GetSelectedOption());
    }
}
