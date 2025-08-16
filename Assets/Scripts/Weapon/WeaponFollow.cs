using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFollow : MonoBehaviour
{
    [SerializeField] private Transform HandPlayer;
    private void Update()
    {
        transform.position = new Vector3(HandPlayer.transform.position.x, HandPlayer.transform.position.y, HandPlayer.transform.position.z);
    }
}
