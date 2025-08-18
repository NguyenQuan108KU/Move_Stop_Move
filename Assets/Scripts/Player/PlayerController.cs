using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
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
    private Enemy enemyCurrent;
    private bool isDetech = false;
    private bool isDead = false;

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

    public bool isGetGift = false;

    //[SerializeField] private Bullet bullet1;
    void Start()
    {
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
        for(int i = 0; i < hatOfPlayer.games.Count(); i++)
        {
            if(indexHats == i)
            {
                hatOfPlayer.games[i].SetActive(true);
            }
            else{
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
        Enemy firstEnemyDetected = null;

        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                firstEnemyDetected = hit.GetComponent<Enemy>();
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
            Debug.Log("abc");
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
        if (isDead)
        {
            AudioManager.instance.StopSFX(0);
        }
        playerMove = Vector3.zero;
        AudioManager.instance.PlayerSFX(0);
        GameObject bulletObj = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetOwner(gameObject);
        bulletScript.SetTarget(target);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet2"))
        {
            AudioManager.instance.isPlayerBGM = false;
            AudioManager.instance.PlayerSFX(0);
            dead1.SetActive(true);
            UIManager.instance.StartDead();
            anim.SetBool("Death", true);
            Harmmer.SetActive(false);
            isDead = true;
            PlayerPrefs.SetInt("coinMoney", coinMoney);
            isPlayerDie = true;
        }

        if (collision.gameObject.CompareTag("Gift"))
        {
            isGetGift = true;
            Destroy(collision.gameObject);
            radius = 6.5f;
            DrawCircle circle = GetComponentInChildren<DrawCircle>();
            if(circle != null)
            {
                circle.radius = 6.5f;
                circle.DrawCircleUnderFeet();
            }
            bullet1.transform.localScale = new Vector3(100, 100, 100);
        }
    }
    public void DestroyPlayer()
    {
        gameObject.SetActive(false);
    }
    public void UpLevel()
    {
        if(countAttack >= 3)
        {
            effectLevelUp.SetActive(true);
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            UIManager.instance.up = 4.5f;
        }
    }
    public void SetDeufalt()
    {
        if (isGetGift)
        {
            isGetGift = false;
            radius = 5f;
            DrawCircle circle = GetComponentInChildren<DrawCircle>();
            if (circle != null)
            {
                circle.radius = 5f;
                circle.DrawCircleUnderFeet();
            }
            bullet1.transform.localScale = new Vector3(39, 39, 39);
        }
    }
}