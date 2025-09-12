using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArrow : MonoBehaviour
{
        public Transform enemy;       // target enemy
        public Transform player;      // trung tâm là player
        public RectTransform arrowUI; // UI mũi tên trên canvas
        public Camera cam;            // Camera chính

        void Update()
        {
            if (enemy == null) return;

            // Chuyển vị trí enemy sang tọa độ màn hình
            Vector3 screenPos = cam.WorldToViewportPoint(enemy.position);

            // Nếu enemy nằm trong màn hình thì ẩn mũi tên
            if (screenPos.z > 0 && screenPos.x > 0 && screenPos.x < 1 && screenPos.y > 0 && screenPos.y < 1)
            {
                arrowUI.gameObject.SetActive(false);
            }
            else
            {
                arrowUI.gameObject.SetActive(true);

                // Vector từ player -> enemy
                Vector3 dir = (enemy.position - player.position).normalized;

                // Góc xoay mũi tên
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                arrowUI.rotation = Quaternion.Euler(0, 0, angle - 90f);

                // Đặt mũi tên ở rìa màn hình
                Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) / 2f;
                Vector3 screenPosWorld = cam.WorldToScreenPoint(player.position + dir * 10f);

                Vector3 fromCenter = (screenPosWorld - screenCenter).normalized;
                float borderOffset = 100f; // khoảng cách từ mép vào trong
                Vector3 arrowPos = screenCenter + fromCenter * (Mathf.Min(screenCenter.x, screenCenter.y) - borderOffset);

                arrowUI.position = arrowPos;
            }
        }
    }
