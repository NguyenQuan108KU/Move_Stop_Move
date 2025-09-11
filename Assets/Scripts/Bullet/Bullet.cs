using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    public Transform target;
    //public WeaponData weaponData;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float speedRotation;
    [SerializeField] private Vector3 shootDirection;
    public float destroyTimer;
    private bool isDestroyed = false; // cờ kiểm soát

    public GameObject owner;
    public bool SetRoration;
    public bool isOffRotate = false;

    [Header("Boomerang")]
    public bool SetRotation = false;
    public bool SetBoomerang = false;
    private Vector3 startPos;
    private bool returning = false;
    [SerializeField] private float maxDistance = 3f; // khoảng cách bay xa nhất

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Start()
    {
        startPos = transform.position; // lưu vị trí ban đầu
        Destroy(gameObject, destroyTimer); // dùng destroyTimer thay vì fix 1f
    }

    public void SetTarget(Transform _target)
    {
        if (_target != null)
        {
            // Chỉ lấy hướng 1 lần rồi lưu lại
            shootDirection = (_target.position - transform.position).normalized;
        }
        else
        {
            // Nếu target null, cho đạn bay thẳng về phía trước
            shootDirection = transform.forward;
        }
    }

    public void SetOwner(GameObject ownerObj)
    {
        owner = ownerObj;
    }

    public void SetDirection(Vector3 dir)
    {
        shootDirection = dir.normalized;
        rb.velocity = shootDirection * bulletSpeed;
    }

    private void FixedUpdate()
    {
        if (SetBoomerang && !SetRoration)
        {
            if (!returning)
            {
                // bay ra phía trước
                rb.velocity = shootDirection * bulletSpeed;
            }
            else
            {
                if (owner != null)
                {
                    // hướng về owner
                    Vector3 dirBack = (owner.transform.position - transform.position).normalized;
                    rb.velocity = dirBack * bulletSpeed;
                }
            }
        }
        else
        {
            // chế độ thường
            rb.velocity = shootDirection * bulletSpeed;
        }
    }
    private void Update()
    {
        if (SetBoomerang && !returning && !SetRoration)
        {
            // nếu vượt quá maxDistance thì quay lại
            if (Vector3.Distance(startPos, transform.position) >= maxDistance)
            {
                returning = true;
            }
        }

        // xử lý xoay như code cũ
        if ((SetRoration || isOffRotate) && !SetBoomerang)
        {
            if (shootDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(rb.velocity.normalized, Vector3.up);
                Vector3 euler = lookRotation.eulerAngles;
                euler.x = -90f;
                transform.rotation = Quaternion.Euler(euler);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(90, 0, Time.time * speedRotation);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == owner)
        {
            // Nếu boomerang quay lại chạm owner thì huỷ
            if (SetBoomerang && returning)
            {
                Destroy(gameObject);
            }
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (GameManager.instance.playerController.isGetGift) return;
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("EnemyController"))
        {
            Destroy(gameObject);
        }
    }
}