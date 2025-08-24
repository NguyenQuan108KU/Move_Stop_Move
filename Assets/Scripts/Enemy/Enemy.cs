using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float EnemySpeed;
    private Animator anim;

    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private Transform firingTransform;
    public Transform target;

    [Header("Detection & Attack Range")]
    [SerializeField] private float detectionRange = 8f;
    [SerializeField] private float attackRange = 3f;

    [Header("Target Tags")]
    [SerializeField] private List<string> targetTags = new List<string> { "Player", "Enemy" };

    public SkinnedMeshRenderer[] render;

    public GameObject BloodParticle;
    public bool isDead = false;
    public GameObject targetEnemy;

    public float timeStartBullet;

    private Vector3 randomDirection;
    private float changeDirectionTime = 3f;
    private float timer = 0f;
    public TextMeshProUGUI textEnemy;

    public TextMeshProUGUI nameEnemy;
    public List<NameData> listName = new List<NameData>();

    public int point;
    public TextMeshProUGUI pointOfEnemy;

    public bool isGetGift = false;

    private void Awake()
    {
        foreach (var item in render)
        {
            item.material.color = Random.ColorHSV();
        }
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        nameEnemy.text = listName[Random.Range(0, listName.Count)].name.ToString();
        
        
    }

    private void Update()
    {
        if (GameManager.instance.playerController.isPlayerDie)
        {
            //anim.SetBool("Idle", true);
            anim.SetBool("Attack", false);
            anim.SetBool("Move", false);
            return;
        }
        EnemyMovement();
        EnemyAttack();
        if (timeStartBullet > 0)
            timeStartBullet -= Time.deltaTime;
        //setVitriScoreEnemy();
        
    }
    private void LateUpdate()
    {
        //setVitriScoreEnemy();
    }
    public void EnemyMovement()
    {
        if (isDead || target != null || GameManager.instance.playerController.isPlayerDie) return; // Không di chuyển nếu đã chết hoặc đang tấn công

        timer += Time.deltaTime;
        if (timer >= changeDirectionTime || CheckWall())
        {
            anim.SetBool("Move", true);
            randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
            timer = 0f;
        }

        Vector3 move = randomDirection * EnemySpeed * Time.deltaTime;
        rb.MovePosition(transform.position + move);

        // Quay mặt theo hướng di chuyển
        if (randomDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(randomDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 5f * Time.deltaTime);
        }
    }

    public void EnemyAttack()
    {
        if (GameManager.instance.playerController.isPlayerDie) return;
        if (isDead) return;

        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        bool foundTarget = false;

        foreach (var hit in colliders)
        {
            if (hit.transform == gameObject.transform) continue;
            if (hit.gameObject.CompareTag("Player") || hit.gameObject.CompareTag("Enemy"))
            {

                target = hit.transform;
                anim.SetBool("Attack", true);
                // Quay mặt về hướng Player
                Vector3 directionEnemy = hit.transform.position - transform.position;
                directionEnemy.y = 0;
                Quaternion toRotation = Quaternion.LookRotation(directionEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);

                foundTarget = true;
                break; // Dừng kiểm tra nếu đã tìm thấy Player
            }
        }

        if (!foundTarget)
        {
            // Player đã rời khỏi phạm vi → quay lại trạng thái di chuyển ngẫu nhiên
            target = null;
            anim.SetBool("Attack", false);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet1"))
        {
            GameManager.instance.playerController.SetDeufalt();
            AudioManager.instance.PlayerSFX(2);
            GameManager.instance.playerController.point += 5;
            GameManager.instance.playerController.coinMoney += 5;
            GameManager.instance.playerController.countAttack += 1;
            UIManager.instance.UpdateAlive();
        }
        if (collision.gameObject.CompareTag("Bullet1") || collision.gameObject.CompareTag("Bullet2"))
        {
            Bullet bulletScript = collision.gameObject.GetComponent<Bullet>();
            if (bulletScript == null) return;
            if (bulletScript.owner == this.gameObject)
            {
                return;
            }
            EnemyManager.instance.enemy = this;
            Enemy enemyShooter = bulletScript.owner.GetComponent<Enemy>();
            if(enemyShooter != null)
            {
                enemyShooter.point += 5;
                enemyShooter.pointOfEnemy.text = enemyShooter.point.ToString();
            }
            else
            {
                GameManager.instance.playerController.point += 5;
                GameManager.instance.playerController.coinMoney += 50;
            }
            
            isDead = true;
            //Praticle System
            BloodParticle.SetActive(true);
            anim.SetBool("Death", true);
            gameObject.tag = "Untagged";
        }

        if (collision.gameObject.CompareTag("Gift"))
        {
            isGetGift = true;
            Destroy(collision.gameObject);
            detectionRange = 12f;
            bulletPrefabs.transform.localScale = new Vector3(100, 100, 100);
        }
    }


    public void Shooting()
    {
        GameObject bulletObj = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.identity);
        bulletObj.tag = "Bullet2"; // tag cho bullet của enemy
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        //Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetOwner(gameObject);
        bulletScript.SetTarget(target);
    }
    void setVitriScoreEnemy()
    {
        Vector3 enemyScreenPosition = Camera.main.WorldToScreenPoint(transform.position + new Vector3(-0.6f, 4, 0));
        textEnemy.rectTransform.position = enemyScreenPosition;
    }
    public void DestroyEnemy() => Destroy(gameObject);
    public bool CheckWall()
    {
        if(Physics.Raycast(transform.position + Vector3.up * 1.0f, transform.forward, out RaycastHit hit, 2f))
        {
            return hit.collider.CompareTag("Wall");
        }
        else
        {
            return false;
        }
    }

    public void SetDeufalt()
    {
        if (isGetGift)
        {
            isGetGift = false;
            detectionRange = 8f;
            bulletPrefabs.transform.localScale = new Vector3(39, 39, 39);
        }
    }
}