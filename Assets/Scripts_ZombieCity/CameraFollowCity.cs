using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowCity : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    public Camera mainCamera;

    private float fovVelocity = 0f; // dùng cho SmoothDamp
    private float targetFOV = 65f; // FOV mặc định

    private void LateUpdate()
    {
        transform.position = new Vector3(
            playerTransform.position.x,
            playerTransform.position.y,
            playerTransform.position.z
        );

        if (GameManager.instance.playerCityController.isSetCircle)
        {
            mainCamera.fieldOfView = Mathf.SmoothDamp(
            mainCamera.fieldOfView,
            targetFOV,
            ref fovVelocity,
            0.8f,
            Mathf.Infinity,
            Time.unscaledDeltaTime
);

        }
    }
}
