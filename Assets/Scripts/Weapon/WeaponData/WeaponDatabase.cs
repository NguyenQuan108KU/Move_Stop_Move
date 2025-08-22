using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class WeaponDatabase : ScriptableObject
{
    public Weapon[] weapon;
    public MaterialOfHammer[] listOfMaterials;

    public int WeaponCount()
    {
        return weapon.Length;
    }
    public Weapon GetWeapon(int index)
    {
        return weapon[index];
    }
  
}