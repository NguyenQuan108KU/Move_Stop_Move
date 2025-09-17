using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("--------------Player Target--------------")]
    public Transform playerTransform;           // Transform của player để camera theo dõi

    [Header("--------------Movement Settings--------------")]
    public float smoothTime = 0.3f;                             // Thời gian mượt khi di chuyển
    private Vector3 velocity = Vector3.zero;                    // Vector hỗ trợ SmoothDamp
    private bool isMovingToStart = true;                        // Kiểm tra camera đang di chuyển về vị trí ban đầu
    private Vector3 targetStartPos = new Vector3(0, 0, -8.4f);  // Vị trí bắt đầu của camera

    [Header("--------------Camera Settings--------------")]
    public Camera mainCamera;           // Camera chính
    private float fovVelocity = 0f;     // Biến dùng cho SmoothDamp FOV
    private float targetFOV = 80f;      // FOV mục tiêu mặc định

    private void LateUpdate(){
        // Nếu tất cả enemy đã chết → camera di chuyển mượt lên vị trí trên player
        if (GameController.instance.enemyTotal <= 0){
            transform.position = Vector3.SmoothDamp(
                transform.position,
                new Vector3(playerTransform.position.x,
                    playerTransform.position.y - 6.5f,
                    playerTransform.position.z + 8.4f),
                ref velocity,
                smoothTime
            );
            return; // Thoát LateUpdate sau khi đã di chuyển
        }

        // Nếu đang di chuyển về vị trí ban đầu
        if (isMovingToStart){
            transform.position = Vector3.SmoothDamp(transform.position, targetStartPos, ref velocity, smoothTime);      // Di chuyển mượt về vị trí ban đầu

            // Khi gần tới vị trí thì lock lại
            if (Vector3.Distance(transform.position, targetStartPos) < 0.05f)
            {
                isMovingToStart = false; // Dừng di chuyển về vị trí ban đầu
            }
        }
        else{
            // Theo dõi player trực tiếp
            transform.position = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y,
                playerTransform.position.z
            );
        }

        // Điều chỉnh FOV dựa trên trạng thái LevelUp của player
        if (GameController.instance != null && GameController.instance.playerController != null){
            if (GameController.instance.playerController.isLevelUp){
                targetFOV = 80f; // FOV khi level up
            }
            else{
                targetFOV = 70f; // FOV bình thường
            }
        }
        mainCamera.fieldOfView = Mathf.SmoothDamp(mainCamera.fieldOfView, targetFOV, ref fovVelocity, 0.6f);            // SmoothDamp FOV → chuyển đổi mượt giữa FOV hiện tại và target
    }
}
