using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadPopupLose : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject loadPopupLose;           // Panel chứa popup
    public TextMeshProUGUI numberOfPopupLose; // Text hiển thị số đếm ngược
    public GameObject ReviveCanvas;            // Canvas Revive
    public GameObject GameOverCanvas;
    public Button skipRevive;// Canvas Game Over

    [Header("Settings")]
    public int countdownTime = 5;              // Thời gian đếm ngược

    private float remainingTime;
    private bool isCounting = false;

    private void Start()
    {
        // Khi Prefab được Instantiate, tự động bật popup và bắt đầu đếm
        ShowPopup();
        skipRevive.onClick.AddListener(EndCountdown);
    }

    private void Update()
    {
        if (!isCounting) return;

        remainingTime -= Time.deltaTime;
        int displayTime = Mathf.CeilToInt(remainingTime);
        numberOfPopupLose.text = displayTime.ToString();

        // Quay icon nếu muốn
        if (loadPopupLose != null)
            loadPopupLose.transform.rotation = Quaternion.Euler(0, 0, Time.time * -100);

        if (remainingTime <= 0)
        {
            EndCountdown();
        }
    }

    private void ShowPopup()
    {
        if (loadPopupLose != null) loadPopupLose.SetActive(true);
        if (ReviveCanvas != null) ReviveCanvas.SetActive(true);
        if (GameOverCanvas != null) GameOverCanvas.SetActive(false);

        remainingTime = countdownTime; // reset thời gian
        isCounting = true;
    }

    private void EndCountdown()
    {
        isCounting = false;

        if (ReviveCanvas != null) ReviveCanvas.SetActive(false);
        if (loadPopupLose != null) loadPopupLose.SetActive(false); // tắt popup nếu muốn
        if (GameOverCanvas != null) GameOverCanvas.SetActive(true);
    }
}
