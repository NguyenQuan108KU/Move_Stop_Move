using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    [Header("--------------Buttons--------------")]
    public Button popUpWinner; // Kéo Button của Prefab vào đây trong Inspector
    public Button popupBackMenu;

    [Header("--------------Prefabs--------------")]
    public GameObject popUpGiftPrefabs;

    private void Awake()
    {
        // Đăng ký sự kiện cho button BackMenu nếu có
        if (popupBackMenu != null)
            popupBackMenu.onClick.AddListener(BackMenu);

        // Đăng ký sự kiện cho button popUpWinner nếu có
        if (popUpWinner != null)
            popUpWinner.onClick.AddListener(DisplayFreeItem);
    }

    // Hiển thị popup quà tặng khi bấm nút Winner
    private void DisplayFreeItem()
    {
        gameObject.SetActive(false); // Ẩn panel hiện tại
        Instantiate(popUpGiftPrefabs, transform.position, Quaternion.identity); // Sinh prefab quà tặng
    }

    // Quay về menu khi bấm nút Back
    private void BackMenu() => GameController.instance.sceneController.BackSceneMenu();
}
