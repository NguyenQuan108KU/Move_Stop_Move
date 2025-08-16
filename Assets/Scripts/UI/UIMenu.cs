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
    void Start()
    {
        Time.timeScale = 1.0f;
        score = PlayerPrefs.GetInt("coinMoney");
        pants.SetPants(pants.LoadPants());
        hair.SetHairs(hair.LoadHats());
        weapon.SetWeapon(weapon.LoadWeapon());
    }

    // Update is called once per frame
    void Update()
    {
        scoreMenu.text = score.ToString();
    }
    public void SaveWeapon()
    {
        PlayerPrefs.SetInt("IndexWeapon", WeaponManager.instance.GetSelectedOption());
    }
}
