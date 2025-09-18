using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyFullSetOfPlayer : MonoBehaviour
{
    public HairManager hatManager;
    public ShieldManager shieldManager;
    public PantsManager pantManager;
    public ClothesManager skinManager;
    public WeaponManager weaponManager;
    private void Start()
    {
        if(DataManager.Ins.gameSave.idSkin != "Skin_2")
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
        weaponManager.LoadColorOfWeapon(PlayerPrefs.GetInt("SelectOption"));
    }
}
