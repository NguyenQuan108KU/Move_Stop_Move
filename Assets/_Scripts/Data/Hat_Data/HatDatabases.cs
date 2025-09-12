using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class HatDatabases : ScriptableObject
{
    public Hat[] hats;   // Thông tin của từng cái mũ 

    //Set index cho từng cái mũ
    private void OnValidate()
    {
        for (int i = 0; i < hats.Length; i++)
        {
            hats[i].index = "Hats_" + i;
        }
    }
}
