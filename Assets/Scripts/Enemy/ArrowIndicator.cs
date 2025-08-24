using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArrowIndicator : MonoBehaviour
{
    public Camera mainCamera;             // Kéo Main Camera vào
    public RectTransform canvasTransform; // Kéo Canvas vào
    public GameObject arrowPrefab;        // Kéo Prefab Arrow vào
    public Enemy enemy;
    private GameObject arrowUI;
    public TextMeshProUGUI text;
    private Image[] images;

    void Start()
    {
        enemy = GetComponent<Enemy>();
        // Tạo mũi tên cho enemy này
        arrowUI = Instantiate(arrowPrefab, canvasTransform);
        images = arrowUI.GetComponentsInChildren<Image>();
        images[0].color = enemy.render[0].material.color; // Image cha
        images[1].color = enemy.render[1].material.color; // Image con (thứ 2)
        text = arrowUI.GetComponentInChildren<TextMeshProUGUI>();
        arrowUI.SetActive(false);
    }

    void Update()
    {
        text.text = enemy.point.ToString();
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);

        // Enemy trong camera
        if (viewportPos.z > 0 && viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1)
        {
            arrowUI.SetActive(false);
        }
        else
        {
            arrowUI.SetActive(true);

            // Vị trí enemy trên màn hình
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(transform.position);
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 dir = new Vector2(screenPoint.x, screenPoint.y) - screenCenter;

            // Nếu enemy ở phía sau camera, đảo hướng
            if (viewportPos.z < 0)
                dir = -dir;

            dir.Normalize();

            // Góc xoay: prefab mặc định hướng lên (0,1)
            float angle = Vector2.SignedAngle(Vector2.up, dir);
            arrowUI.transform.rotation = Quaternion.Euler(0, 0, angle);
            images[1].rectTransform.rotation = Quaternion.identity;

            // Đặt mũi tên trên viền màn hình (cách mép 50px)
            float halfWidth = Screen.width / 2f - 50;
            float halfHeight = Screen.height / 2f - 50;

            float slope = (dir.x != 0) ? dir.y / dir.x : float.MaxValue;
            Vector2 edgePos = screenCenter;

            if (Mathf.Abs(slope) < halfHeight / halfWidth) // chạm viền trái/phải
            {
                edgePos.x += dir.x > 0 ? halfWidth : -halfWidth;
                edgePos.y += slope * (edgePos.x - screenCenter.x);
            }
            else // chạm viền trên/dưới
            {
                edgePos.y += dir.y > 0 ? halfHeight : -halfHeight;
                edgePos.x += (edgePos.y - screenCenter.y) / slope;
            }

            arrowUI.GetComponent<RectTransform>().position = edgePos;
        }
    }

    void OnDestroy()
    {
        if (arrowUI != null)
            Destroy(arrowUI);
    }
}
