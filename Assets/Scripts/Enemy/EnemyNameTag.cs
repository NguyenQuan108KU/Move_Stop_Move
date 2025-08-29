using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyNameTag : MonoBehaviour
{
    public Camera mainCamera;            // Camera chính
    public RectTransform nameTagPrefab;  // Prefab Text
    public Canvas mainCanvas;            // Canvas ngoài

    private RectTransform nameTagUI;

    void Start()
    {
        // Khi enemy spawn thì tạo 1 text trong canvas
        nameTagUI = Instantiate(nameTagPrefab, mainCanvas.transform);

        // Đổi nội dung text nếu cần
        //nameTagUI.GetComponent<TextMeshProUGUI>().text = gameObject.name;
    }

    void LateUpdate()
    {
        if (mainCamera == null || nameTagUI == null) return;

        // Lấy vị trí enemy trên màn hình
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position + Vector3.up * 3f);

        // Gán lại vị trí cho UI
        nameTagUI.position = screenPos;
    }

    private void OnDestroy()
    {
        // Khi enemy chết thì xoá luôn nameTag
        if (nameTagUI != null)
            Destroy(nameTagUI.gameObject);
    }
}
