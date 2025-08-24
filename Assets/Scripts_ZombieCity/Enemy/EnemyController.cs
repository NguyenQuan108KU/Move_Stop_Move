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
    void Start()
    {
        target = GameManager.instance.playerCityController.transform;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Tắt xoay tự động để tự xoay bằng script
        agent.updateRotation = false;
        SetColorEnemy();
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
        ParticleSystem ps = praticleSystem.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = listColors.colors[indexColor].material.color;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet1"))
        {
            AudioManager.instance.PlayerSFX(2);
            GameManager.instance.playerCityController.EnemyAlive -= 1;
            GameManager.instance.playerCityController.coinOfPlayer += 5;

            // Spawn particle riêng biệt tại vị trí enemy
            GameObject ps = Instantiate(praticleSystem, transform.position, Quaternion.identity);
            var particle = ps.GetComponent<ParticleSystem>();
            particle.Play();

            Destroy(ps, particle.main.duration + particle.main.startLifetime.constantMax); // tự hủy sau khi chạy xong

            Destroy(gameObject); // xóa enemy
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            isDead = true;
            anim.SetBool("Move", false);
        }
    }
}
