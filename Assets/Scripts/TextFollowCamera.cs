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

    void Update()
    {
        // luôn nhìn thẳng vào camera
        transform.LookAt(
            transform.position + mainCam.transform.rotation * Vector3.forward,
            mainCam.transform.rotation * Vector3.up
        );

        // giữ nguyên kích thước (không bị phóng to/thu nhỏ khi camera di chuyển)
        transform.localScale = initialScale;
    }
}
