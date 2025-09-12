using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ShieldDatabases : ScriptableObject
{
    public Shield[] shields;   // Thông tin của từng cái khiên 

    //Set index cho từng cái khiên
    private void OnValidate()
    {
        for (int i = 0; i < shields.Length; i++)
        {
            shields[i].index = "Shiled_" + i;
        }
    }
}
