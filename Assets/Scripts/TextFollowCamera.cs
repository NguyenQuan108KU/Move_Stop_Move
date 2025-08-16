using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFollowCamera : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 initialScale;

    void Start()
    {
        mainCam = Camera.main;
        initialScale = transform.localScale; // lưu kích thước ban đầu
    }

    void LateUpdate()
    {
        // luôn nhìn thẳng vào camera
        transform.LookAt(
            transform.position + mainCam.transform.rotation * Vector3.forward,
            mainCam.transform.rotation * Vector3.up
        );

        // giữ nguyên kích thước
        transform.localScale = initialScale;
    }


}
