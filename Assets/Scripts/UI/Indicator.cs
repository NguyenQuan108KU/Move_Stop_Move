using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    public GameObject IndicatorS;   // mũi tên UI
    public Transform Target;        // enemy
    public Camera mainCam;          // camera chính
    private SkinnedMeshRenderer mesh;

    void Start()
    {
        mesh = GetComponent<SkinnedMeshRenderer>();
    }

    void Update()
    {
        if (!mesh.isVisible) // enemy không trong camera
        {
            IndicatorS.SetActive(true);
        }
        else
        {
            IndicatorS.SetActive(false);
        }
    }
}
