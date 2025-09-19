using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Enemy : MonoBehaviour
{
    [Header("------------------Enemy Components------------------")]
    public Rigidbody rb;            // Rigidbody của enemy
    public Animator anim;           // Animator điều khiển enemy

    public GameObject bulletPrefabs;          // Prefab đạn của enemy
    public Transform firingTransform;         // Vị trí bắn đạn
    public Transform target;                  // Mục tiêu hiện tại
    public Color[] possibleColors;            // danh sách màu bạn set sẵn trên Inspector
    private static List<Color> availableColors = new List<Color>();
    public Color pinkColor;
    public Color chosenColor { get; private set; }

    [Header("------------------Detection & Attack Range------------------")]
    public float detectionRange = 8f;         // Phạm vi phát hiện
    public float attackRange = 3f;            // Phạm vi tấn công
    public float attackCoolDown = 3.0f;
    public float attackTimer;

    [Header("------------------Visual & Effects------------------")]
    public SkinnedMeshRenderer[] render;                        // Renderer để đổi màu enemy
    public GameObject BloodParticle;                            // Hiệu ứng máu khi enemy chết
    public GameObject circleTargetEnemy;                        //Vòng tròn mục tiêu dưới chân enemy 

    [Header("------------------Visual & Effects------------------")]
    public bool isEnemyDied = false;        // Cờ kiểm tra enemy đã chết chưa
    public bool isEnemyGetGift = false;          // Cờ enemy có nhặt gift chưa
    public bool isEnemyAttacking = false;        // Cờ kiểm soát enemy đang tấn công
    public bool hasBeenEnemyHit = false;        // Cờ đã bị bắn trúng chưa
    public bool isEnemyHit;

    [Header("------------------Movement Randomizer------------------")]
    private Vector3 randomDirection;
    public float EnemySpeed;        // Tốc độ di chuyển
    public float timeStartBullet;

    [Header("------------------UI Elements------------------")]
    public TextMeshProUGUI textEnemy;           // Text hiển thị trạng thái enemy
    public TextMeshProUGUI nameEnemy;           // Text hiển thị tên enemy
    public Image imagePoint;
    public TextMeshProUGUI pointOfEnemy;        // Text hiển thị điểm của enemy
    public List<NameData> listName = new List<NameData>();      // Danh sách tên enemy
    public int point;   // Điểm số của enemy
    public static int colorIndex = 0;
    public GameObject informationOfEnemy;

    [Header("------------------Ground Check------------------")]
    public GameObject GroundCheck;          // Điểm kiểm tra mặt đất (dùng raycast)
    private float changeDirectionTime = 1.5f;
    private float timer = 0f;
    [Header("------------------Floating Text------------------")]
    public GameObject floatingTextPrefab; // kéo prefab vào trong Inspector


    private void Awake(){
        SetColorOfEnemy();
    }

    //Hàm set màu cho enemy

    private void Start()
    {
        hasBeenEnemyHit = false;
        nameEnemy.text = listName[Random.Range(0, listName.Count)].name.ToString();
        attackTimer = attackCoolDown;
        detectionRange = 8f;
        bulletPrefabs.transform.localScale = new Vector3(39, 39, 39);
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;
        if (GameController.instance.playerController.isDead)
        {
            anim.SetBool("Attack", false);
            anim.SetBool("Move", false);
            return;
        }
        EnemyAttack();
        if (!isEnemyAttacking)  // thay vì !isAttacking
        {
            EnemyMovement();
        }
        if (timeStartBullet > 0)
            timeStartBullet -= Time.deltaTime;
    }
    public void SetColorOfEnemy()
    {
        if (possibleColors.Length == 0) return;

        // Nếu danh sách trống, refill
        if (availableColors.Count == 0)
            availableColors = new List<Color>(possibleColors);

        // Bốc random 1 màu trong danh sách
        int index = Random.Range(0, availableColors.Count);
        Color baseColor = availableColors[index];
        chosenColor = new Color(baseColor.r, baseColor.g, baseColor.b, 1f); // ép alpha = 1
        availableColors.RemoveAt(index); // loại bỏ để không bị trùng

        // Gán màu
        foreach (var item in render)
            item.material.color = chosenColor;

        nameEnemy.color = chosenColor;
        imagePoint.color = chosenColor;

        var ps = BloodParticle.GetComponent<ParticleSystem>().main;
        ps.startColor = chosenColor;
    }


    //Hàm Enemy di chuyển 
    public void EnemyMovement(){
        // Nếu enemy đã chết, đang có target (tấn công) hoặc player đã chết → không di chuyển
        if (isEnemyDied || target != null || GameController.instance.playerController.isDead) return;
        // Thay đổi hướng nếu hết thời gian, gặp tường hoặc không còn ground phía trước
        timer += Time.deltaTime;
        if (timer >= changeDirectionTime || EnemyCheckWall() || !EnemyCheckGround()){
            anim.SetBool("Move", true); // Kích hoạt animation di chuyển
            randomDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;   // Tạo hướng di chuyển ngẫu nhiên
            timer = 0f;
        }
        // Di chuyển enemy theo hướng randomDirection
        Vector3 move = randomDirection * EnemySpeed * Time.deltaTime;
        rb.MovePosition(transform.position + move);

        // Quay mặt theo hướng di chuyển nếu có hướng
        if (randomDirection != Vector3.zero){
            Quaternion toRotation = Quaternion.LookRotation(randomDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 5f * Time.deltaTime); // Quay mượt
        }
    }

    //Hàm enemy tấn công người chơi 
    public void EnemyAttack(){
        // Nếu player đã chết hoặc enemy đã chết thì thoát hàm
        if (GameController.instance.playerController.isDead || isEnemyDied) return;

        // Lấy tất cả collider xung quanh enemy trong bán kính detectionRange
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);

        bool foundTarget = false; // Biến kiểm tra xem đã tìm thấy target chưa

        // Duyệt tất cả collider
        foreach (var hit in colliders){
            if (hit.transform == gameObject.transform) continue; // Bỏ qua chính enemy này

            // Nếu collider là Player hoặc Enemy khác
            if (hit.gameObject.CompareTag("Player") || hit.gameObject.CompareTag("Enemy")){
                // Nếu attackTimer <= 0 → có thể tấn công
                if (attackTimer <= 0){
                    target = hit.transform;            // Cập nhật target
                    rb.velocity = Vector3.zero;        // Dừng enemy
                    rb.angularVelocity = Vector3.zero; // Dừng quay
                    anim.SetBool("Attack", true);      // Kích hoạt animation tấn công
                    isEnemyAttacking = true;                // Đánh dấu trạng thái tấn công
                    Vector3 directionEnemy = hit.transform.position - transform.position;   // Quay mặt về hướng Player
                    directionEnemy.y = 0;   // Giữ nguyên chiều cao
                    Quaternion toRotation = Quaternion.LookRotation(directionEnemy);
                    transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);

                    foundTarget = true; // Đã tìm thấy target
                    break; // Dừng kiểm tra
                }
            }
        }

        // Nếu không tìm thấy target trong phạm vi
        if (!foundTarget){
            target = null;               // Reset target
            anim.SetBool("Attack", false); // Tắt animation tấn công, quay lại trạng thái di chuyển
        }
    }

    //Hàm enemy bắn đạn 
    public void Shooting(){
        // Tạo viên đạn mới từ prefab tại vị trí firingTransform
        GameObject bulletObj = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.identity);
        bulletObj.tag = "Bullet2"; // Gán tag cho bullet của enemy

        Bullet bulletScript = bulletObj.GetComponent<Bullet>(); // Lấy component Bullet từ viên đạn
        bulletScript.SetOwner(gameObject); // Thiết lập owner là enemy
        bulletScript.SetTarget(target);     // Thiết lập target là player hoặc mục tiêu

        // Nếu enemy đang có trạng thái gift
        if (isEnemyGetGift){
            StartCoroutine(ScaleBullet(bulletObj, 39f, 100f, 1.0f)); // Scale viên đạn từ 39 lên 100 trong 1 giây
            detectionRange = 12f; // Tăng phạm vi phát hiện
        }
        else{
            bulletObj.transform.localScale = new Vector3(39, 39, 39); // Scale bình thường
            detectionRange = 8f; // Phạm vi phát hiện mặc định
        }

        attackTimer = attackCoolDown; // Reset thời gian cooldown tấn công
        StartCoroutine(AttackEnd()); // Bắt đầu Coroutine kết thúc trạng thái tấn công(tránh trường hợp khi vừa ở trạng thái di chuyển vừa tấn công) 
    }

    //Hàm tăng kích thước bullet dần dân to lên khi ăn quà 
    private IEnumerator ScaleBullet(GameObject bullet, float startScale, float endScale, float duration){
        float elapsedTime = 0f; // Thời gian đã trôi qua

        // Vòng lặp chạy trong khoảng thời gian duration
        while (elapsedTime < duration){
            elapsedTime += Time.deltaTime; // Cộng dồn thời gian trôi qua
            float scale = Mathf.Lerp(startScale, endScale, elapsedTime / duration);          //Tămg scale từ startScale → endScale
            bullet.transform.localScale = new Vector3(scale, scale, scale); // Áp dụng scale đồng đều cho 3 trục
            yield return null; 
        }
        bullet.transform.localScale = new Vector3(endScale, endScale, endScale); // Đảm bảo scale cuối chính xác
    }

    //Hàm check va chạm với tường enemy 
    public bool EnemyCheckWall(){
        // Raycast từ vị trí enemy lên 1 unit, về phía trước, khoảng cách 2f
        if (Physics.Raycast(transform.position + Vector3.up * 1.0f, transform.forward, out RaycastHit hit, 2f))
            return hit.collider.CompareTag("Wall");     // Nếu va chạm với wall → trả về true
        else
            return false;           // Không va chạm → trả về false
    }

    //Hàm check va chạm với măt đất enemy 
    public bool EnemyCheckGround(){
        // Bắn ray từ GroundCheck xuống dưới 2 đơn vị
        if (Physics.Raycast(GroundCheck.transform.position, Vector3.down, out RaycastHit hit, 2f))   // Nếu va chạm với collider có tag "Ground" → trả về true
            return hit.collider.CompareTag("Ground");
        else
            return false; // Không va chạm → trả về false
    }

    //Hàm reset đạn về trạng thái mặc định sau khi ăn quà 
    public void SetDeufaltBullet(){
        // Nếu enemy đang trong trạng thái gift
        if (isEnemyGetGift)
        {
            isEnemyGetGift = false; // Reset trạng thái gift
            detectionRange = 8f; // Đặt phạm vi phát hiện về mặc định
            bulletPrefabs.transform.localScale = new Vector3(39, 39, 39); // Reset scale viên đạn về mặc định
        }
    }

    //Hàm kết thúc tấn công (gán ở event animation)
    public IEnumerator AttackEnd()
    {
        yield return new WaitForSeconds(0.5f); // Chờ 0.5 giây trước khi kết thúc trạng thái tấn công
        isEnemyAttacking = false; // Kết thúc trạng thái tấn công
    }
    //Hàm tắt enemy (gọi ở event của animation)
    public void DestroyEnemy() => Destroy(gameObject);

    //Hàm enemy khi bị tấn công 
    public void OnHit(){
        // Làm tối đi 50% so với màu gốc
        Color darkened = chosenColor * 0.65f;
        darkened.a = 1f; // giữ alpha = 1
        foreach (var item in render)
            item.material.color = darkened;
        informationOfEnemy.SetActive(false);
        anim.SetBool("Death", true);                                // Kích hoạt animation chết
        AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Die);  // Đánh dấu enemy đã chết
        GameController.instance.uiManager.UpdateAliveEnemy();       // Cập nhật số lượng enemy còn sống
        BloodParticle.SetActive(true);                              // Hiển thị particle máu
    }
    private void OnCollisionEnter(Collision collision){
        // Nếu va chạm với bullet1 (của player)
        if (collision.gameObject.CompareTag("Bullet1") && !isEnemyDied){
            if (floatingTextPrefab != null)
            {
                GameObject ft = Instantiate(
                floatingTextPrefab,
                GameController.instance.playerController.transform.position + Vector3.up * 2f,
                Quaternion.identity,
                GameController.instance.playerController.transform // parent chính là Player
);
                ft.GetComponent<FloatingText>().Setup("+1", Color.white);
            }
            // Nếu player đang có gift, bỏ qua va chạm giữa bullet và enemy
            //if (GameController.instance.playerController.isGetGift) {
                //Collider enemyCollider = GetComponent<Collider>();
                //Collider bulletCollider = collision.collider;                   // collider của viên đạn
                //if (enemyCollider != null && bulletCollider != null)
                //    Physics.IgnoreCollision(enemyCollider, bulletCollider);     // Bỏ qua va chạm
            //}
            OnHit();
            GameController.instance.playerController.SetBulletPlayerDeufalt();
            GameController.instance.playerController.pointOfPlayerDefault += 1;      // Player nhận điểm
            GameController.instance.playerController.coinMoney += 50;               // Player nhận coin
            GameController.instance.uiManager.UpdateCoin();                         //Cập nhật coin
            GameController.instance.uiManager.UpdatePoint();
            isEnemyDied = true;
        }

        //Khi va chạm với đạn của enemy bắn ra 
        if (collision.gameObject.CompareTag("Bullet2") && !isEnemyDied){
            Bullet bulletScript = collision.gameObject.GetComponent<Bullet>();
            if (bulletScript == null) return;

            //// Nếu viên đạn thuộc về chính enemy này → bỏ qua
            if (bulletScript.owner == gameObject) return;
            Enemy enemyShooter = bulletScript.owner.GetComponent<Enemy>();          // Lấy enemy bắn viên đạn
            if (enemyShooter != null){
                enemyShooter.point += 5; // Tăng điểm cho enemy bắn
                enemyShooter.pointOfEnemy.text = enemyShooter.point.ToString(); // Cập nhật UI điểm
            }
            OnHit();
            isEnemyDied = true;
        }
        // Nếu va chạm với gift
        if (collision.gameObject.CompareTag("Gift")){
            isEnemyGetGift = true;
            Destroy(collision.gameObject);
            //detectionRange = 12f;
            //bulletPrefabs.transform.localScale = new Vector3(100, 100, 100);
        }
    }
}