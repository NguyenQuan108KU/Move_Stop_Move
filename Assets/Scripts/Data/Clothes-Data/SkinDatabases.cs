using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class SkinDatabases : ScriptableObject
{
    public Skin[] skin;
    private void OnValidate()
    {
        for(int i  = 0; i < skin.Length; i++)
        {
            skin[i].index = "Skin_" + i;
        }
    }
}
