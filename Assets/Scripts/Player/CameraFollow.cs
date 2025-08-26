using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;

    private bool isMovingToStart = true;
    private Vector3 targetStartPos = new Vector3(0, 0, -8.4f);

    public Camera mainCamera;
    private float fovVelocity = 0f; // dùng cho SmoothDamp
    private float targetFOV = 80f; // FOV mặc định
    private float yVelocity = 0f; // cho SmoothDamp y

    private void LateUpdate()
    {
        if (isMovingToStart)
        {
            // Di chuyển mượt về vị trí ban đầu
            transform.position = Vector3.SmoothDamp(transform.position, targetStartPos, ref velocity, smoothTime);

            // Khi gần tới vị trí thì lock lại
            if (Vector3.Distance(transform.position, targetStartPos) < 0.05f)
            {
                isMovingToStart = false;
            }
        }
        else
        {
            // Theo dõi player
            transform.position = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                playerTransform.position.z
            );
        }
        if (GameManager.instance.playerController.isLevelUp)
        {
            targetFOV = 80f;
            //Vector3 camPos = mainCamera.transform.position;
            //camPos.y = 12f;
            //mainCamera.transform.position = camPos;
        }
        else
        {
            targetFOV = 70f; // hoặc FOV ban đầu
        }

        // SmoothDamp FOV
        mainCamera.fieldOfView = Mathf.SmoothDamp(mainCamera.fieldOfView, targetFOV, ref fovVelocity, 1.4f);
    }
}
