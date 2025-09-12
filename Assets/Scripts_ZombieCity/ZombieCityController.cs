using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCityController : MonoBehaviour
{
    public static ZombieCityController instance;
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }
}
