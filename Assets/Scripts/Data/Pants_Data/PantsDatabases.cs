using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class PantsDatabases : ScriptableObject
{
    public Pants[] pants;   // Thông tin của từng cái quần 

    //Set index cho từng cái quần
    private void OnValidate(){
        for(int i = 0; i < pants.Length; i++)
        {
            pants[i].index = "Pants_" + i;
        }
    }
}
