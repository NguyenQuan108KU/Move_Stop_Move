using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    [Header("Property")]
    [SerializeField] private List_Color listColors;
    private Animator anim;
    public float lookRadius = 10.0f;
    public float rotationSpeed = 5f; // Tốc độ xoay

    Transform target;
    NavMeshAgent agent;

    [Header("Praticle System")]
    [SerializeField] private GameObject praticleSystem;
    [Header("Material Enemy")]
    [SerializeField] private GameObject colorEnemy;    public GameObject targetEnemy;    public bool isDead = false;    public bool isGetGift = false;

    public bool isBoss;
    public bool isBossLevel1;    public bool isBossLevel2;    public int countAttack;    public GameObject hatColor;
    void Start()
    {
        target = GameManager.instance.playerCityController.transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Tắt xoay tự động để tự xoay bằng script
        agent.updateRotation = false;
        SetColorEnemy();
        SetColorHair();
        if (isBossLevel1)
        {
            countAttack = 2;
        }
        else if(isBossLevel2)
        {
            countAttack = 4;
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        if (GameManager.instance.playerCityController.isOffPlayer || isDead)
        {
            agent.ResetPath();                  // Xóa đường đi của NavMeshAgent
            anim.SetBool("Move", false);        // Tắt animation Move
            return;                             // Thoát khỏi Update
        }
        if (distance <= lookRadius && !isDead)
        {
            anim.SetBool("Move", true);
            agent.SetDestination(target.position);

            // Luôn xoay về phía player khi trong phạm vi
            FaceTarget();

            // Nếu trong khoảng dừng thì có thể làm hành động tấn công
            if (distance <= agent.stoppingDistance)
            {
                // Ví dụ: Attack
            }
        }
        else
        {
            anim.SetBool("Move", false);
        }
    }

    private void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
    public void SetColorEnemy()
    {
        int indexColor = Random.Range(0, listColors.colors.Length);
        colorEnemy.GetComponent<SkinnedMeshRenderer>().material = listColors.colors[indexColor].material;
    }
    public void SetColorHair()
    {
        if (hatColor != null)
        {
            int indexColor = Random.Range(0, listColors.colors.Length);
            hatColor.GetComponent<MeshRenderer>().material = listColors.colors[indexColor].material;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet1"))
        {
            if (isBoss)
            {
                countAttack -= 1;
                if(countAttack <= 0)
                {
                    EnemyDie();
                }
            }
            else
            {
                EnemyDie();
            }
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            isDead = true;
            anim.SetBool("Move", false);
        }
    }
    public void EnemyDie()
    {
        GameObject effect = Instantiate(praticleSystem);
        effect.transform.rotation = Quaternion.identity;
        effect.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        effect.GetComponent<ParticleSystem>().Play();

        AudioManager.instance.PlayerSFX(2);
        GameManager.instance.playerCityController.EnemyAlive -= 1;
        GameManager.instance.playerCityController.coinOfPlayer += 1;
        Destroy(gameObject); // xóa enemy
    }
}
