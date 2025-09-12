using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieCityController : MonoBehaviour
{
    public static ZombieCityController instance;
    public PlayerCityController playerCityController;
    public UIManager uiManager;
    public SceneController sceneController;

    [Header("---------------Check Win or Lose------------------")]
    public GameObject popupWin;
    public GameObject popupLose;
    public int zombieTotal;        //Tổng số enemy
    public bool isCheckWinLose;   //Check xem cos win lose không
    private void Awake(){
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }
    private void Start(){
        playerCityController.Init();
    }
    private void Update(){
        CheckWinOrLoseCity();
        // Nếu player đã chết thì ngừng update
        if (playerCityController.isDead) return;
        playerCityController.PlayerMove();
        playerCityController.AttackTrigle();
        //Tăng level player
        playerCityController.UpLevel();
        playerCityController.SetWinner();
        // ================== Xử lý Khiên Bảo Vệ ==================
        if (playerCityController.circlePtotect.activeSelf)
        {
            playerCityController.timerProtectPlayer += Time.deltaTime;
            // Sau 3s thì tắt khiên
            if (playerCityController.timerProtectPlayer >= 3)
            {
                playerCityController.circlePtotect.SetActive(false);
                playerCityController.timerProtectPlayer = 0;
                playerCityController.countProtect -= 1;    // Giảm số lần bảo vệ còn lại
            }
            // Nếu không còn lượt bảo vệ thì tắt trạng thái
            if (playerCityController.countProtect <= 0)
                playerCityController.isProtectPlayer = false;
        }
        // ================== Xử lý Vũ khí xoay khi load game ==================
        if (playerCityController.weaponLoadOfMenu != null)
            playerCityController.WeaponRotateWhenStartGame();
    }
    private void CheckWinOrLoseCity(){
        if (ZombieCityController.instance.playerCityController.isDead && !isCheckWinLose){
            isCheckWinLose = true;
            Instantiate(popupLose, transform.position, Quaternion.identity);
        }else if(zombieTotal <=0  && !isCheckWinLose){
            isCheckWinLose = true;
            Instantiate(popupWin, transform.position, Quaternion.identity);
        }
    }
}
