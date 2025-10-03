using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ApplyFullSetOfPlayer : MonoBehaviour
{
    public HairManager hatManager;
    public ShieldManager shieldManager;
    public PantsManager pantManager;
    public ClothesManager skinManager;
    public WeaponManager weaponManager;
    public TextMeshProUGUI textOfPlayer;
    public int coinOfPlayer;
    private void Start()
    {
        SetCoinPlayer();
        if (DataManager.Ins.gameSave.idSkin != "Skin_2")
        {
            hatManager.isSetHat = false;
            shieldManager.isSetShield = false;
            pantManager.isSetPant = false;
            skinManager.isSetClothes = true;
            skinManager.SetSkinOfPlayer();
        }
        else
        {
            shieldManager.SetShieldOfPlayer();
            pantManager.SetPaintOfPlayer();
            hatManager.SetHatOfPlayer();
        }
        weaponManager.OptionWeaponWhenStartGame();
        weaponManager.LoadColorOfWeapon(PlayerPrefs.GetInt("SelectOption"));
        weaponManager.SetWeaponStartGame();
    }
    public void SetCoinPlayer()
    {
        coinOfPlayer = 100000;
        textOfPlayer.text = coinOfPlayer.ToString();
    }
}
