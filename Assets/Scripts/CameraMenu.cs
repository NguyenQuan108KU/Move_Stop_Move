using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMenu : MonoBehaviour
{
    private Vector3 targetCamera;   // Vị trí đích hiện tại
    private Vector3 defaultCamera;  // Vị trí mặc định
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        defaultCamera = transform.position;
        targetCamera = defaultCamera;
    }

    void Update()
    {
        // Di chuyển dần về target mỗi frame
        transform.position = Vector3.SmoothDamp(transform.position, targetCamera, ref velocity, smoothTime);
    }

    public void CameraShop()
    {
        targetCamera = new Vector3(6.9f, -1.5f, -21.29f);
    }

    public void SetCameraDefault()
    {
        targetCamera = defaultCamera;
    }
}
