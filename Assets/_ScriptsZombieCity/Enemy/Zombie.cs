using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{

    [Header("--------------Enemy Properties--------------")]
    public Rigidbody rb;
    public List_Color listColors;     // Danh sách màu để random màu cho enemy
    public Animator anim;                              // Animator điều khiển animation
    public float lookRadius = 10.0f;                    // Bán kính phát hiện target
    public float rotationSpeed = 5f;                    // Tốc độ xoay về phía target
    Transform target;                                   // Target hiện tại (player)
    NavMeshAgent agent;                                 // Agent để enemy di chuyển tự động

    [Header("--------------Visual & Effects--------------")]
    public GameObject praticleSystemEnemyDie; // Hiệu ứng khi enemy chết
    [Header("--------------Enemy Appearance--------------")]
    public GameObject colorEnemy;     // Đối tượng thay đổi màu (mesh enemy)
    public GameObject hatColor;                         // Đối tượng thay đổi màu của mũ
    public GameObject circleTargetZombie;               // Vòng tròn hiển thị khi bị chọn làm target
    [Header("--------------Enemy States--------------")]
    public bool isDead = false;                         // Enemy đã chết chưa
    public bool isGetGift = false;                      // Enemy có rơi quà khi chết không

    [Header("--------------Boss Config--------------")]
    public bool isBoss;            // Có phải boss không
    public bool isBossLevel1;      // Boss cấp 1
    public bool isBossLevel2;      // Boss cấp 2            
    public bool isBossLevel3;      // Boss cấp 3
    public int countAttackZombie;  // Số lần chịu đòn trước khi chết (boss)

    public GameObject floatingTextPrefab; // kéo prefab vào trong Inspector
    public int poinOdBoss;   //Điểm khi giết chết enemy
    void Start(){
        target = ZombieCityController.instance.playerCityController.transform;
        agent = GetComponent<NavMeshAgent>();

        // Tắt xoay tự động để tự xoay bằng script
        agent.updateRotation = false;
        SetColorEnemy();
        SetColorHair();
        if (isBossLevel1)
        {
            countAttackZombie = 2;
        }
        else if(isBossLevel2 || isBossLevel3)
        {
            countAttackZombie = 4;
            poinOdBoss = 5;
        }
        else
        {
            poinOdBoss = 1;
        }
        
    }

    public void ZombieUpdate(){
        float distance = Vector3.Distance(transform.position, target.position);
        if (ZombieCityController.instance.playerCityController.isOffPlayer || isDead){
            agent.ResetPath();                  // Xóa đường đi của NavMeshAgent
            anim.SetBool("Move", false);        // Tắt animation Move
            return;                             // Thoát khỏi Update
        }
        if (distance <= lookRadius && !isDead){
            anim.SetBool("Move", true);
            agent.SetDestination(target.position);

            // Luôn xoay về phía player khi trong phạm vi
            ZombieFaceTarget();

            // Nếu trong khoảng dừng thì có thể làm hành động tấn công
            if (distance <= agent.stoppingDistance){
                // Ví dụ: Attack
            }
        }
        else{
            anim.SetBool("Move", false);
        }
    }

    // Quay nhân vật hướng về target
    private void ZombieFaceTarget(){
        Vector3 direction = (target.position - transform.position).normalized;          // Lấy vector hướng từ player đến target

        // Nếu có hướng hợp lệ (tránh lỗi khi direction = (0,0,0))
        if (direction != Vector3.zero){
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));            // Tạo quaternion xoay theo hướng (chỉ quay theo trục XZ, bỏ qua Y)
            transform.rotation = Quaternion.Slerp(          // Quay mượt từ góc hiện tại sang góc mới bằng Slerp
                transform.rotation,
                lookRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    /// Đặt màu ngẫu nhiên cho Enemy (toàn bộ mesh).
    public void SetColorEnemy(){
        int indexColor = Random.Range(0, listColors.colors.Length);             // Lấy chỉ số màu ngẫu nhiên trong danh sách
        colorEnemy.GetComponent<SkinnedMeshRenderer>().material = listColors.colors[indexColor].material;           // Gán material tương ứng cho SkinnedMeshRenderer của Enemy
    }

    public void UpdatePointOfPlayer()
    {
        GameObject ft = Instantiate(
                floatingTextPrefab,
                ZombieCityController.instance.playerCityController.transform.position + Vector3.up * 2f,
                Quaternion.identity,
                ZombieCityController.instance.playerCityController.transform);
        ft.GetComponent<FloatingText>().Setup("+" + poinOdBoss, Color.white);
    }
    // Đặt màu ngẫu nhiên cho nón/tóc của Enemy.
    public void SetColorHair(){
        if (hatColor != null){
            int indexColor = Random.Range(0, listColors.colors.Length);
            hatColor.GetComponent<MeshRenderer>().material = listColors.colors[indexColor].material;
        }
    }
    private void OnCollisionEnter(Collision collision){
        // Khi Enemy va chạm với đạn của Player (Bullet1)
        if (collision.gameObject.CompareTag("Bullet1")){
         
            // Nếu Enemy là Boss
            if (isBoss){
                // Nếu là Boss level 3 → cơ chế thu nhỏ dần khi trúng đạn
                if (isBossLevel3){
                    // Giảm kích thước boss
                    Vector3 newScale = transform.localScale - new Vector3(0.5f, 0.5f, 0.5f);
                    transform.localScale = newScale;
                    // Nếu boss đã nhỏ hơn kích thước tối thiểu → chết
                    if (transform.localScale.x < 1f || transform.localScale.y < 1f || transform.localScale.z < 1f){
                        UpdatePointOfPlayer();
                        AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Die);
                        ZombieCityController.instance.uiManager.UpdatePoinPlayerCity();
                        ZombieCityController.instance.uiManager.UpdateAliveZombie();
                        ZombieCityController.instance.playerCityController.pointOfPlayerCity += poinOdBoss;
                        EnemyDie();
                    }
                }
                else{
                    countAttackZombie -= 1;       // Boss thường → giảm số lần chịu đòn
                    // Nếu đã hết máu/đòn chịu được → chết
                    if (countAttackZombie <= 0){
                        AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Die);
                        ZombieCityController.instance.playerCityController.pointOfPlayerCity += poinOdBoss;
                        ZombieCityController.instance.uiManager.UpdatePoinPlayerCity();
                        ZombieCityController.instance.uiManager.UpdateAliveZombie();
                        EnemyDie();
                    }
                }
            }
            else{
                // Enemy thường → chết ngay khi trúng đạn
                UpdatePointOfPlayer();
                AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Die);
                ZombieCityController.instance.playerCityController.pointOfPlayerCity += poinOdBoss;
                ZombieCityController.instance.uiManager.UpdatePoinPlayerCity();
                ZombieCityController.instance.uiManager.UpdateAliveZombie();
                EnemyDie();
            }
        }

        // Khi Enemy va chạm trực tiếp với Player
        if (collision.gameObject.CompareTag("Player")){
            rb.isKinematic = true;
            isDead = true;                  
            anim.SetBool("Move", false);    // Tắt animation di chuyển
        }
    }
    public void EnemyDie() {
        ZombieCityController.instance.zombies.Remove(this);
        // Tạo hiệu ứng particle khi enemy chết
        GameObject effect = Instantiate(praticleSystemEnemyDie);
        effect.transform.rotation = Quaternion.identity; // đặt rotation mặc định
        effect.transform.position = new Vector3(
            transform.position.x,
            transform.position.y + 0.5f,
            transform.position.z
        ); // dịch lên một chút cho đẹp
        effect.GetComponent<ParticleSystem>().Play(); // chạy particle

        // Giảm số lượng zombie đang sống trong game
        //ZombieCityController.instance.playerCityController.zombieAlive -= 1;
        if (!isDead)
        {
            ZombieCityController.instance.zombieTotal -= 1;
            isDead = true;
        }

        // Tăng số coin của player khi hạ enemy
        ZombieCityController.instance.playerCityController.coinOfPlayerCity += 1;

        // Xóa enemy ra khỏi scene
        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
