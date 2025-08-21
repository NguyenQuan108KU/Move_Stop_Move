using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCity_Controller : MonoBehaviour
{
    public Animator anim;
    public Joystick joystick;

    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private Transform firingTransform;

    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    private Vector3 playerMove;

    [Header("Radius")]
    [SerializeField] private float radius;

    [SerializeField] private GameObject Harmmer;
    [SerializeField] private Transform target;
    private bool isAttack = false;
    float timer;
    [SerializeField] private float attackDuration = 1f; // thời gian duy trì trạng thái attack
    private float attackTimer = 0f;
    public int point;
    private EnemyController enemyCurrent;
    private bool isDetech = false;
    public bool isDead = false;

    public GameObject dead1;
    public int coinMoney;

    [Header("Change Weapon")]
    public Test test;
    public GameObject weaponChoose;
    public Bullet bullet1;
    private int indexWeapon;
    public bool isPlayerDie = false;

    public int countAttack;
    public GameObject effectLevelUp;
    [Header("Change Pants")]
    [SerializeField] private int indexPants;
    [SerializeField] private ListPants listPants;
    [SerializeField] private GameObject pantsOdPlayer;

    [Header("Change Hats")]
    [SerializeField] private int indexHats;
    [SerializeField] private HATS hatOfPlayer;

    [Header("Attack Settings")]
    [SerializeField] private float fireRate = 0.5f; // thời gian chờ giữa các lần bắn
    private float nextFireTime = 0f;

    public int EnemyAlive;
    public GameObject winner;
    public int coinOfPlayer;

    public bool isOffPlayer = false;

    //[SerializeField] private Bullet bullet1;
    void Start()
    {
        coinOfPlayer = 0;
        EnemyAlive = 10;
        point = 0;
        coinMoney = PlayerPrefs.GetInt("coinMoney");
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (isDead) return;
        PlayerMove();
        AttackTrigle();
        //Thay doi quan ao, vu khi
        changeWepon();
        changePants();
        changeHats();
        //Len level
        UpLevel();
        SetWinner();
    }
    void changeWepon()
    {
        indexWeapon = PlayerPrefs.GetInt("IndexWeapon");
        for (int i = 0; i < test.list.Count(); i++)
        {
            if (test.list[i].index == indexWeapon)
            {
                weaponChoose.GetComponent<MeshFilter>().mesh = test.list[i].weaponMesh;
                bullet1.GetComponent<MeshFilter>().mesh = test.list[i].weaponMesh;
                if (test.list[i].isRotate)
                {
                    bullet1.SetRoration = true;
                }
                else
                {
                    bullet1.SetRoration = false;
                }
            }
        }
    }
    void changePants()
    {

        indexPants = PlayerPrefs.GetInt("IndexPants");
        for (int i = 0; i < listPants.pantsObjects.Count(); i++)
        {
            if (listPants.pantsObjects[i].index == indexPants)
            {
                pantsOdPlayer.GetComponent<SkinnedMeshRenderer>().material = listPants.pantsObjects[i].materialPants;
            }
        }
    }
    void changeHats()
    {
        indexHats = PlayerPrefs.GetInt("IndexHat");
        for (int i = 0; i < hatOfPlayer.games.Count(); i++)
        {
            if (indexHats == i)
            {
                hatOfPlayer.games[i].SetActive(true);
            }
            else
            {
                hatOfPlayer.games[i].SetActive(false);
            }
        }
    }
    private void PlayerMove()
    {
        if (isAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                isAttack = false;
                anim.SetBool("Attack", false);
                Harmmer.SetActive(true);
                target = null;
                attackTimer = 0f;
            }
            else
            {
                anim.SetFloat("Speed", 0);
            }
        }
        else
        {
            playerMove.x = joystick.Horizontal;
            playerMove.z = joystick.Vertical;
            playerMove.y = 0;

            Vector3 movement = playerMove * moveSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);
            anim.SetFloat("Speed", playerMove.sqrMagnitude);

            if (playerMove.sqrMagnitude > 0.01f)
            {
                Quaternion toRotation = Quaternion.LookRotation(playerMove, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    public void AttackTrigle()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        EnemyController firstEnemyDetected = null;

        foreach (var hit in colliders)
        {
            if (hit.CompareTag("EnemyController"))
            {
                firstEnemyDetected = hit.GetComponent<EnemyController>();
                break; // chỉ lấy enemy đầu tiên
            }
        }

        if (firstEnemyDetected != null)
        {
            if (enemyCurrent != null && enemyCurrent != firstEnemyDetected)
            {
                enemyCurrent.targetEnemy.SetActive(false); // Tắt enemy cũ nếu khác
            }

            enemyCurrent = firstEnemyDetected;
            enemyCurrent.targetEnemy.SetActive(true); // Bật enemy mới
            if (playerMove.sqrMagnitude == 0.0f)
            {
                target = enemyCurrent.transform;
                anim.SetBool("Attack", true);
                Vector3 directionEnemy = target.position - transform.position;
                directionEnemy.y = 0;
                Quaternion toRotation = Quaternion.LookRotation(directionEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }
            else
            {
                anim.SetBool("Attack", false);
            }
        }
        else
        {
            if (enemyCurrent != null)
            {
                enemyCurrent.targetEnemy.SetActive(false);
                enemyCurrent = null;
            }
        }
    }

    public void SetOffAttack() => anim.SetBool("Attack", false);
    public void Shooting()
    {
        if (Time.time >= nextFireTime) // chỉ bắn khi đã hết cooldown
        {
            nextFireTime = Time.time + fireRate; // đặt lại thời gian bắn tiếp theo
            AudioManager.instance.PlayerSFX(0);
            playerMove = Vector3.zero;
            GameObject bulletObj = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.identity);
            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
            bulletScript.SetTarget(target);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet2"))
        {
            //AudioManager.instance.isPlayerBGM = false;
            //AudioManager.instance.PlayerSFX(0);
            dead1.SetActive(true);
            UIManager.instance.StartDead();
            anim.SetBool("Death", true);
            Harmmer.SetActive(false);
            isDead = true;
            PlayerPrefs.SetInt("coinMoney", coinMoney);
            isPlayerDie = true;
        }
        if (collision.gameObject.CompareTag("EnemyController"))
        {
            //AudioManager.instance.PlayerSFX(3);
            dead1.SetActive(true);
            UIManager.instance.StartDead();
            isDead = true;
            anim.SetBool("Death", true);
            isOffPlayer = true;
        }
    }
    public void DestroyPlayer()
    {
        gameObject.SetActive(false);
    }
    public void UpLevel()
    {
        if (countAttack >= 1)
        {
            effectLevelUp.SetActive(true);
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            UIManager.instance.up = 4.5f;
        }
    }
    public void SetWinner()
    {
        if(EnemyAlive <= 0)
        {
            EnemyAlive = 0;
            winner.SetActive(true);
            StartCoroutine(StopGameAfterDelay(2f));
        }
    }
    private IEnumerator StopGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Time.timeScale = 0f;
    }
}
