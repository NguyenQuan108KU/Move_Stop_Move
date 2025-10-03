using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public PlayerController playerController;
    public Enemy enemy;
    public UIManager uiManager;
    public SceneController sceneController;

    public int enemyTotal;

    public GameObject popupWin;
    public GameObject popupLose;
    public GameObject joystick;
    public bool isChecckWinLose;
    public List<Enemy> enemies = new List<Enemy>();     // quản lý nhiều enemy

    private void Awake(){
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }
    private void Start(){
        AdsController.Instance.ShowInterstitial();
        playerController.Init();
    }
    private void Update() {
        CheckWinOrLose();
        playerController.UpdatePlayer();

        // Update tất cả Enemy
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].EnemyUpdate();
        }
    }
    private void CheckWinOrLose(){
        if(enemyTotal <= 0 && !isChecckWinLose){
            isChecckWinLose = true;
            Instantiate(popupWin, transform.position, Quaternion.identity);
            joystick.SetActive(false);
            
        }
        else if (playerController.isDead && !isChecckWinLose)
        {
            Instantiate(popupLose, transform.position, Quaternion.identity);
            isChecckWinLose = true;
            joystick.SetActive(false);
        }
    }
}
