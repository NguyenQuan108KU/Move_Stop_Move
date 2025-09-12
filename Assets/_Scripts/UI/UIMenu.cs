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
    public ShieldManager protect;

    public TextMeshProUGUI coinOfPlayerText;
    void Start()
    {
        Time.timeScale = 1.0f;
        score = PlayerPrefs.GetInt("coinMoney");
        weapon.SetWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        coinOfPlayerText.text = score.ToString();
        scoreMenu.text = score.ToString();
    }
}
