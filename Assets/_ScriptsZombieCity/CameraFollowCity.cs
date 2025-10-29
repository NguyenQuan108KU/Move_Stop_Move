using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraFollowCity : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    public Camera mainCamera;
    [Header("--------------Movement Settings--------------")]
    public float smoothTime = 0.3f;                             // Thời gian mượt khi di chuyển
    private Vector3 velocity = Vector3.zero;

    private float fovVelocity = 0f; // dùng cho SmoothDamp
    public float targetFOV; // FOV mặc định
    public bool isCheckSetCircle;
    private void Start()
    {
        int sizeCircle = PlayerPrefs.GetInt("RangeAttack");
        if (sizeCircle > 0)
            isCheckSetCircle = true;
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (isCheckSetCircle)
            {
                if (ZombieCityController.instance.playerCityController.sizeCircle == 10)
                    targetFOV = 64;
                else if (ZombieCityController.instance.playerCityController.sizeCircle == 20)
                    targetFOV = 67;
                else
                    targetFOV = 70;
                mainCamera.fieldOfView = Mathf.SmoothDamp(
                    mainCamera.fieldOfView,
                    targetFOV,
                    ref fovVelocity,
                    0.3f,
                    Mathf.Infinity,
                    Time.unscaledDeltaTime
                );
            }
        }
    }
    private void LateUpdate()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int sceneIndex = currentScene.buildIndex;

        // Nếu đã thắng thì di chuyển mượt đến vị trí thắng
        if (GameController.instance != null && GameController.instance.enemyTotal <= 0)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                new Vector3(
                    playerTransform.position.x,
                    playerTransform.position.y - 6.5f,
                    playerTransform.position.z + 8.4f
                ),
                ref velocity,
                smoothTime
            );
            return; // Không set lại vị trí camera nữa
        }
        if (ZombieCityController.instance != null && ZombieCityController.instance.zombieTotal <= 0)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                new Vector3(
                    playerTransform.position.x,
                    playerTransform.position.y - 5f,
                    playerTransform.position.z + 8.4f
                ),
                ref velocity,
                smoothTime
            );
            return; // Không set lại vị trí camera nữa
        }

        // Bình thường thì follow player
        transform.position = new Vector3(
            playerTransform.position.x,
            playerTransform.position.y,
            playerTransform.position.z
        );

        if (sceneIndex != 0)
        {
            if (ZombieCityController.instance.playerCityController.isSetCircle)
            {
                if (ZombieCityController.instance.playerCityController.sizeCircle == 10)
                    targetFOV = 64;
                else if (ZombieCityController.instance.playerCityController.sizeCircle == 20)
                    targetFOV = 67;
                Debug.Log("Quan cam");
                mainCamera.fieldOfView = Mathf.SmoothDamp(
                    mainCamera.fieldOfView,
                    targetFOV,
                    ref fovVelocity,
                    0.3f,
                    Mathf.Infinity,
                    Time.unscaledDeltaTime
                );
            }
        }
        if(sceneIndex == 0)
        {
            if (GameController.instance != null && GameController.instance.playerController != null)
            {
                if (GameController.instance.playerController.isLevelUp)
                {
                    targetFOV = 80f; // FOV khi level up
                }
                else
                {
                    targetFOV = 70f; // FOV bình thường
                }
            }
            if (GameController.instance != null && GameController.instance.playerController != null)
            {
                if (GameController.instance.playerController.isGetGift && !GameController.instance.playerController.isReturnCamera)
                {
                    int zoom = GameController.instance.playerController.levelUp;
                    if (zoom == 1)
                        targetFOV = 80f;
                    else
                        targetFOV = 85f;// FOV khi level up
                }
            }
            mainCamera.fieldOfView = Mathf.SmoothDamp(mainCamera.fieldOfView, targetFOV, ref fovVelocity, 0.3f);            // SmoothDamp FOV → chuyển đổi mượt giữa FOV hiện tại và target
        }
    }
    public void SetSefaultCamera()
    {
        if (GameController.instance != null && GameController.instance.playerController != null)
        {
            targetFOV = 70f; // FOV bình thường
        }
        mainCamera.fieldOfView = Mathf.SmoothDamp(mainCamera.fieldOfView, targetFOV, ref fovVelocity, 0.3f);
    }
}
