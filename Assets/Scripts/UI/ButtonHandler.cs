using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public Button popUpWinner; // Kéo Button của Prefab vào đây trong Inspector
    public Button popupBackMenu;
    public GameObject popUpGiftPrefabs;

    private void Awake()
    {
        
        if (popupBackMenu != null)  popupBackMenu.onClick.AddListener(BackMenu);
        if (popUpWinner != null) popUpWinner.onClick.AddListener(DisplayFreeItem);
    }
    private void DisplayFreeItem()
    {
        gameObject.SetActive(false);
        Instantiate(popUpGiftPrefabs, transform.position, Quaternion.identity);
    }
    private void BackMenu() => SceneController.instance.BackSceneMenu();
}
