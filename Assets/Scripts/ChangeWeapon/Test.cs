using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="abc")]
public class Test : ScriptableObject
{
    public ChooseWeapon[] list;

    public int ListCount()
    {
        return list.Length;
    }

}
