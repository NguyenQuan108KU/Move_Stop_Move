using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Load : MonoBehaviour
{
    [SerializeField] public GameObject loadCircle;
    [SerializeField] public float speedRotation;
    private void Update()
    {
        loadCircle.transform.rotation = Quaternion.Euler(0, 0, Time.time * speedRotation);
    }
}
