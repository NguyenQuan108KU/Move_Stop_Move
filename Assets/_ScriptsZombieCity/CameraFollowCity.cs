using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        Scene currentScene = SceneManager.GetActiveScene();     // Lấy scene hiện tại
        int sceneIndex = currentScene.buildIndex;
        if(sceneIndex != 4)
        {
            if (ZombieCityController.instance.playerCityController.isSetCircle)
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
}
