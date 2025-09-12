using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    [Header("--------------Player Info UI--------------")]
    public TextMeshProUGUI textEnemyAlive;
    public TextMeshProUGUI pointOfPlayerCity;
    public TextMeshProUGUI pointOfPlayerDefault;
    public TextMeshProUGUI namePlayer;
    public TextMeshProUGUI coinOfPlayer;
    public int enemyAliveTotal;

    [Header("--------------Enemy Info UI--------------")]
    [Header("--------------Loading UI--------------")]
    [Header("--------------Dead UI--------------")]
    [SerializeField] public GameObject loadCircle;
    [SerializeField] public GameObject loadGift;
    [SerializeField] public float speedRotation;
    [SerializeField] TextMeshProUGUI number;
    [SerializeField] public bool isDead = false;
    public TextMeshProUGUI coinOfPlayerDefault;
    public GameObject Winner;

    public float up = 4;

    public TextMeshProUGUI enemyCity;

    public GameObject GiftOpen;

    public GameObject nameOfPlayer;
    public GameObject circleOfPlayer;

    [Header("-----------------")]
    public TextMeshProUGUI readyCity;
    public float numberOfReadyCity = 3;
    public GameObject menuReady;
    public List<GameObject> spawn;
    public bool isLoadMenu;

    private void Start()
    {
        isLoadMenu = false;
        enemyAliveTotal = 8;
    }
    private void Update()
    {
        //if (coin != null && GameManager.instance.playerController != null)
        //    coin.text = GameManager.instance.playerController.coinMoney.ToString();
        //if (enemyCity != null)
        //    enemyCity.text = GameManager.instance.playerCityController.EnemyAlive.ToString();
        //if (coinOfPlayer != null)
        //    coinOfPlayer.text = GameManager.instance.playerCityController.coinOfPlayer.ToString();
        //textEnemyAlive.text = enemyAliveTotal.ToString();
        //if(GameManager.instance.playerController != null)
        //{
        //    pointOfPlayerCity.text = GameManager.instance.playerController.pointOfPlayerDefault.ToString();
        //}
        //if (GameManager.instance.playerCityController != null)
        //{
        //    pointOfPlayerCity.text = GameManager.instance.playerCityController.point.ToString();
        //}
        if(loadGift != null)
            loadGift.transform.rotation = Quaternion.Euler(0, 0, Time.time * -speedRotation);
        if((readyCity != null || menuReady != null) && isLoadMenu)
        {
            LoadCity();
        }
        UpdateAliveZombie();
    }
    public void UpdateCoin()
    {
        if (coinOfPlayerDefault != null && GameController.instance.playerController != null)
            coinOfPlayerDefault.text = GameController.instance.playerController.coinMoney.ToString();
        //if (coinOfPlayer != null)
        //    coinOfPlayer.text = ZombieCityController.instance.playerCityController.coinOfPlayerCity.ToString();
    }
    public void UpdatePoint()
    {
        if (GameController.instance.playerController != null)
        {
            pointOfPlayerDefault.text = GameController.instance.playerController.pointOfPlayerDefault.ToString();
        }
    }
    public void UpdatePoinPlayerCity(){
        if (ZombieCityController.instance.playerCityController != null)
        {
            pointOfPlayerCity.text = ZombieCityController.instance.playerCityController.pointOfPlayerCity.ToString();
        }
    }
    public void UpdateAliveEnemy()
    {
        GameController.instance.enemyTotal -= 1;
        textEnemyAlive.text = GameController.instance.enemyTotal.ToString();
        if (GameController.instance.enemyTotal <= 0)
        {
            AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Win);
            GameController.instance.enemyTotal = 0;
            nameOfPlayer.SetActive(false);
            circleOfPlayer.SetActive(false);
            GameController.instance.playerController.anim.SetTrigger("Dancer");
            //StartCoroutine(StopGameAfterDelay(1f)); // gọi coroutine sau 1s
        }
    }
    public void UpdateAliveZombie()
    {
        if (enemyCity != null)
            enemyCity.text = ZombieCityController.instance.zombieTotal.ToString();
    }

    private IEnumerator StopGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // chờ 1 giây
        Time.timeScale = 0;
    }
    public void LoadCity()
    {
        numberOfReadyCity -= Time.deltaTime;
        if(numberOfReadyCity < 0)
        {
            menuReady.SetActive(false);
            foreach(var i in spawn)
            {
                i.SetActive(true); 
            }
        }
        readyCity.text = (Mathf.RoundToInt(numberOfReadyCity)).ToString();
    }
    public void SetBoolMenu() => isLoadMenu = true;
    public void StopGame() => Time.timeScale = 0;
    public void ContinueGame() => Time.timeScale = 1;
    public void SetGift() => Instantiate(GiftOpen, transform.position, Quaternion.identity);
}
