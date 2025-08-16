using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    public Transform target;
    public WeaponData weaponData;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float speedRotation;
    [SerializeField] private Vector3 shootDirection;
    public float destroyTimer;

    public GameObject owner;
    public bool SetRoration;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 1f);
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
        shootDirection = (target.position - transform.position).normalized;
    }
    public void SetOwner(GameObject ownerObj)
    {
        owner = ownerObj;
    }

    private void FixedUpdate()
    {
        if (!target) return;
        rb.velocity = shootDirection * bulletSpeed;
    }
    private void Update()
    {
        //transform.rotation = Quaternion.Euler(90, 0, Time.time * speedRotation);
        if(SetRoration)
        {
            if (!target) return;
            Vector3 direction = target.position - transform.position;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation * Quaternion.Euler(90, 0, 0);
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
            Destroy(gameObject, 0.35f);
        }
    }
}