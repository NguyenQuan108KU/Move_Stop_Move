using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEnemy : MonoBehaviour
{
    Transform mainCam;
    Transform unit;
    Transform worldSpaceCanvas;

    public Vector3 offset;

    private void Start()
    {
        mainCam = Camera.main.transform;
        unit = transform.parent;
        worldSpaceCanvas = GameObject.FindAnyObjectByType<Canvas>().transform;

        transform.SetParent(worldSpaceCanvas);
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - mainCam.transform.position);
        transform.position = unit.position + offset;
    }
}
