using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

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

    private void Awake(){
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }
    private void Start(){
        playerController.Init();
    }
    private void Update() {
        CheckWinOrLose();
        if (playerController.isDead) return;
        playerController.PlayerMove();
        playerController.AttackTrigle();
        playerController.UpLevel();
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
        }
    }
}
