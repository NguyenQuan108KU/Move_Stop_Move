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

    public GameObject owner;
    public bool SetRoration;

    private bool velocitySet = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        
        Destroy(gameObject, 1f);
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
        //if (!target) return;
        rb.velocity = shootDirection * bulletSpeed;
    }
    private void Update()
    {
        if (SetRoration)
        {
            // Xoay theo hướng bay đã tính
            if (shootDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(shootDirection);
                transform.rotation = lookRotation * Quaternion.Euler(-90, 0, 0);
            }
        }
        else
        {
            transform.rotation = Quaternion.Euler(90, 0, Time.time * speedRotation);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == owner) return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("EnemyController"))
        {
            Destroy(gameObject);
        }
    }
}