using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TextMeshProUGUI alive;
    public TextMeshProUGUI pointOfPlayer;
    public TextMeshProUGUI namePlayer;
    public int enemyAliveTotal;

    [Header("Load")]
    [SerializeField] public GameObject loadCircle;
    [SerializeField] public GameObject loadGift;
    [SerializeField] public float speedRotation;
    [SerializeField] TextMeshProUGUI number;
    private int countNumber;

    [Header("Dead2")]
    public GameObject Canvas_Dead_1;
    public GameObject Canvas_Dead_2;
    [SerializeField] public bool isDead = false;
    private int countdownDuration;
    private float deadStartTime;

    public TextMeshProUGUI coin;

    public GameObject Winner;

    public float up = 4;

    public TextMeshProUGUI enemyCity;
    public TextMeshProUGUI coinOfPlayer;

    public GameObject GiftOpen;


    private void Start()
    {
        enemyAliveTotal = 3;
        countNumber = 5;

        countdownDuration = 5;
        deadStartTime = Time.time;
    }
    private void Update()
    {
        if (coin != null && GameManager.instance.playerController != null)
            coin.text = GameManager.instance.playerController.coinMoney.ToString();
        if (enemyCity != null)
            enemyCity.text = GameManager.instance.playerCityController.EnemyAlive.ToString();
        if (coinOfPlayer != null)
            coinOfPlayer.text = GameManager.instance.playerCityController.coinOfPlayer.ToString();
        alive.text = enemyAliveTotal.ToString();
        //setVitriScorePlayer();
        if(GameManager.instance.playerController != null)
        {
            pointOfPlayer.text = GameManager.instance.playerController.point.ToString();
        }
        if (GameManager.instance.playerCityController != null)
        {
            pointOfPlayer.text = GameManager.instance.playerCityController.point.ToString();
        }

        if (isDead)
        {
            Load();
        }
        //Load();
        if(loadGift != null)
            loadGift.transform.rotation = Quaternion.Euler(0, 0, Time.time * -speedRotation);
    }
    private void LateUpdate()
    {
        //setVitriScorePlayer();
    }
    public void UpdateAlive()
    {
        enemyAliveTotal -= 1;
        if (enemyAliveTotal <= 0 && !Canvas_Dead_1.activeSelf && !Canvas_Dead_2.activeSelf)
        {
            enemyAliveTotal = 0;
            Winner.SetActive(true);
            StartCoroutine(StopGameAfterDelay(1f)); // gọi coroutine sau 1s
        }
    }

    private IEnumerator StopGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // chờ 1 giây
        Time.timeScale = 0;
    }
    void setVitriScorePlayer()
    {
        Vector3 enemyScreenPosition = Camera.main.WorldToScreenPoint(GameManager.instance.playerController.transform.position + new Vector3(0, up, 0));
        namePlayer.transform.position = enemyScreenPosition;
    }
    //Load khi player chết

    public void StartDead()
    {
        isDead = true;
        deadStartTime = Time.time; // lưu lại thời điểm chết
    }
    public void Load()
    {
        //Canvas_Dead_1.SetActive(true);
        loadCircle.transform.rotation = Quaternion.Euler(0, 0, Time.time * -speedRotation);
        int count = countdownDuration - Mathf.FloorToInt(Time.time - deadStartTime);
        number.text = count.ToString();
        if(count <= 0)
        {
            Canvas_Dead_1.SetActive(false);
            Canvas_Dead_2.SetActive(true);
        }
    }
    public void StopGame() => Time.timeScale = 0;
    public void ContinueGame() => Time.timeScale = 1;
    public void SaveGift() => PlayerPrefs.SetInt("Gift", 1);
    public void SetGift() => GiftOpen.SetActive(true);
}
