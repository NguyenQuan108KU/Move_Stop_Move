using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="NewWeaponData", menuName = "Weapon/WeaponData")]
public class WeaponData :  ScriptableObject
{
    //private Rigidbody rb;
    public string tag;
    public Transform target;
    //[SerializeField] private float bulletSpeed;
    //[SerializeField] private float speedRotation;
}
