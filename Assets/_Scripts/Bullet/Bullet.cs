using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rb;
    public Transform target;
    public CapsuleCollider capsualColider;
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
    [SerializeField] private float maxDistance = 8f; // khoảng cách bay xa nhất

    private void Awake(){
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Start(){
        startPos = transform.position; // lưu vị trí ban đầu
        if (!SetBoomerang)
        {
            // chỉ tự hủy với đạn thường
            StartCoroutine(AutoDestroyAfterDelay(1f));
        }
    }

    public void SetTarget(Transform _target)
    {
        if (_target != null)
        {
            // lấy hướng chính xác theo firingTransform thay vì chỉ tính từ vị trí
            shootDirection = (_target.position - transform.position).normalized;
        }
        else
        {
            shootDirection = transform.forward; // fallback
        }
        rb.linearVelocity = shootDirection * bulletSpeed; // set velocity ngay khi spawn
    }
    public void SetOwner(GameObject ownerObj){
        owner = ownerObj;
    }

    public void SetDirection(Vector3 dir){
        shootDirection = dir.normalized;
        rb.linearVelocity = shootDirection * bulletSpeed;
    }

    private void FixedUpdate(){
        if (SetBoomerang && !SetRoration) {
            if (!returning) {
                // bay ra phía trước
                rb.linearVelocity = shootDirection * bulletSpeed;
            }
            else{
                if (owner != null)
                {
                    // hướng về owner
                    Vector3 dirBack = (owner.transform.position - transform.position).normalized;
                    rb.linearVelocity = dirBack * bulletSpeed;
                }
            }
        }
        else {
            // chế độ thường
            rb.linearVelocity = shootDirection * bulletSpeed;
        }
    }
    private void Update() {
        if (SetBoomerang && !returning && !SetRoration) {
            // nếu vượt quá maxDistance thì quay lại
            if (Vector3.Distance(startPos, transform.position) >= maxDistance){
                returning = true;
            }
        }

        // xử lý xoay như code cũ
        if ((SetRoration || isOffRotate) && !SetBoomerang) {
            if (shootDirection != Vector3.zero) {
                Quaternion lookRotation = Quaternion.LookRotation(rb.linearVelocity.normalized, Vector3.up);
                Vector3 euler = lookRotation.eulerAngles;
                euler.x = -90f;
                if (GameController.instance.playerController.isCheckBoomerang && gameObject.tag == "Bullet1")
                    euler.z -= 110.0f;
                transform.rotation = Quaternion.Euler(euler);
            }
        }
        else {
            transform.rotation = Quaternion.Euler(90, 0, Time.time * speedRotation);
        }
    }
    private IEnumerator AutoDestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject == owner){
            // Nếu boomerang quay lại chạm owner thì huỷ
            if (SetBoomerang && returning){
                Destroy(gameObject);
            }
            return;
        }

        if (collision.gameObject.CompareTag("Enemy")){
            Debug.Log("isGetGift = " + GameController.instance.playerController.isGetGift);
            if (GameController.instance.playerController.isGetGift)
            {
                Debug.Log("Destroy");
                capsualColider.isTrigger = true;
            }
            else
            {
            Debug.Log("UnDestroy");
            Destroy(gameObject);
            }
        }

        if (collision.gameObject.CompareTag("EnemyController")) {
            Destroy(gameObject);
        }
    }
}