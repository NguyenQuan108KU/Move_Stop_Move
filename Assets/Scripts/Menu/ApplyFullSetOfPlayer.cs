using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyFullSetOfPlayer : MonoBehaviour
{
    public HairManager hatManager;
    public ProtectManager shieldManager;
    public PantsManager pantManager;
    public ClothesManager skinManager;
    private void Start()
    {
        if(DataManager.Ins.gameSave.idSkin != "Skin_2")
        {
            hatManager.isSetHat = false;
            shieldManager.isSetShield = false;
            pantManager.isSetPant = false;
            skinManager.isResetClothes = true;
            skinManager.SetSkinOfPlayer();
        }
        else
        {
            shieldManager.SetShieldOfPlayer();
            pantManager.SetPaintOfPlayer();
            hatManager.SetHatOfPlayer();
        }
    }
}
