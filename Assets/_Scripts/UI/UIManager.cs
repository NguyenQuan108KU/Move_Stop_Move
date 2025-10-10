using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("--------------Player Info UI--------------")]
    public TextMeshProUGUI pointOfPlayerCity;               // Điểm khi chơi City
    public TextMeshProUGUI pointOfPlayerDefault;            // Điểm mặc định của Player
    public TextMeshProUGUI coinOfPlayer;                    // Coin (UI HUD)
    public TextMeshProUGUI coinOfPlayerDefault;             // Coin (UI màn Default)
    public GameObject nameOfPlayer;                         // Tên Player

    [Header("--------------Enemy Info UI--------------")]
    public TextMeshProUGUI textEnemyAlive;                  // Enemy còn sống
    public TextMeshProUGUI textEnemyAliveCity;              // Zombie còn sống trong City
    [Header("--------------Loading UI--------------")]
    public GameObject loadCircle;          // Vòng tròn loading
    public GameObject loadGift;            // Vòng tròn gift
    public float speedRotation;            // Tốc độ quay
    public TextMeshProUGUI readyCity;                       
    public float timeOfReadyCity = 3;                     // Thời gian Ready City
    [Header("--------------Dead UI--------------")]
    public TextMeshProUGUI number;
    //SerializeField] public bool isDead = false;
    public GameObject menuReady;
    public List<GameObject> activeSpanwEnemy;               //Bật spawn enemy khi load xong  
    public bool isLoadMenu;
    [Header("--------------GAME STATE UI--------------")]
    public GameObject GiftOpen;         // Gift khi mở quà
    public float up = 4;
    public GameObject circleOfPlayer;
    private void Start(){
        isLoadMenu = false;     // chưa load menu
    }
    private void Update(){
        //if (loadGift != null)
        //    loadGift.transform.rotation = Quaternion.Euler(0, 0, Time.time * -speedRotation);
        //Load vào game Zombie city
        UpdateAliveZombie();
        if ((readyCity != null || menuReady != null) && isLoadMenu) {
            LoadCity();
        }
        //UpdateAliveZombie();
    }
    public void UpdateCoin(){
        // Update UI số coin nếu tồn tại text + player controller
        if (coinOfPlayerDefault != null && GameController.instance.playerController != null)
            coinOfPlayerDefault.text = GameController.instance.playerController.coinMoney.ToString();
    }
    public void UpdatePoint(){
        // Update UI điểm player mặc định
        if (GameController.instance.playerController != null){
            pointOfPlayerDefault.text = GameController.instance.playerController.pointOfPlayerDefault.ToString();
        }
    }
    public void UpdatePoinPlayerCity(){
        // Update UI điểm player trong City
        if (ZombieCityController.instance.playerCityController != null){
            pointOfPlayerCity.text = ZombieCityController.instance.playerCityController.pointOfPlayerCity.ToString();
        }
    }
    public void UpdateAliveEnemy() {
        // Giảm số lượng enemy còn lại và update UI
        GameController.instance.enemyTotal -= 1;
        textEnemyAlive.text = GameController.instance.enemyTotal.ToString();
        if (GameController.instance.enemyTotal <= 0){
            AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Win);
            GameController.instance.enemyTotal = 0;
            nameOfPlayer.SetActive(false);
            circleOfPlayer.SetActive(false);
            GameController.instance.playerController.anim.SetTrigger("Dancer");
            //StartCoroutine(StopGameAfterDelay(1f)); // gọi coroutine sau 1s
        }
    }
    public void UpdateAliveZombie(){
        // Cập nhật UI số zombie trong city nếu có enemyCity text
        if (textEnemyAliveCity != null)
            textEnemyAliveCity.text = ZombieCityController.instance.zombieTotal.ToString();
    }

    private IEnumerator StopGameAfterDelay(float delay){
        yield return new WaitForSeconds(delay); // chờ 1 giây
        Time.timeScale = 0;
    }
    //Hàm load quay vũ khí khi vào game 
    public void LoadCity(){
        timeOfReadyCity -= Time.deltaTime;
        if(timeOfReadyCity < 0){
            menuReady.SetActive(false);
            foreach(var i in activeSpanwEnemy){
                i.SetActive(true); 
            }
        }
        readyCity.text = (Mathf.RoundToInt(timeOfReadyCity)).ToString();
    }

    // Bật cờ load menu
    public void SetBoolMenu() => isLoadMenu = true;

    // Dừng game
    public void StopGame() => Time.timeScale = 0;

    // Tiếp tục game
    public void ContinueGame() => Time.timeScale = 1;

    // Spawn gift tại vị trí hiện tại
    public void SetGift() => Instantiate(GiftOpen, transform.position, Quaternion.identity);
}
