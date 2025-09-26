using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerCityController : MonoBehaviour
{
    [Header("------------------Player Components------------------")]
    public Rigidbody rb;
    public Animator anim;                   // Animator của nhân vật
    public Animator animTextOfLevelUp;      // Joystick chữ khi lên cấp
    public GameObject textOfAnimLevelUp;    // Chữ khi lên cấp
    public Joystick joystick;               // Joystick điều khiển nhân vật

    [Header("------------------Bullet------------------")]
    public GameObject bulletPrefabs;        // Prefabs của viên đạn
    public Transform firingTransform;       // Vị trí viên đạn được bắn ra

    [Header("------------------Move Info------------------")]
    public float moveSpeedOfPlayerCity;      // Tốc độ di chuyển của nhân vật
    private Vector3 directionOfPlayerCity;   // Hướng di chuyển của nhân vật dựa trên joystick

    [Header("------------------Radius------------------")]
    public float radiusAttackOfPlayerCity;   // Bán kính vòng tròn phát hiện Enemy của Player


    public GameObject weaponOfPlayerCity;    // Vũ khí của nhân vật 
    private Transform target;                // Vị trí của Enemy
    private bool isAttack = false;           // Cho biết xem có tấn công hay không
    public float attackDuration = 1f;        // thời gian duy trì trạng thái attack
    private float attackTimer = 0f;
    public int pointOfPlayerCity;            // Điểm của người chơi
    private Zombie enemyCurrent;
    public bool isDead = false;              // Kiểm tra xem nhân vật đã chết hay chưa
    public int coinMoney;                    // Tiền của người chơi

    public GameObject dead1;

    [Header("------------------Change Weapon------------------")]
    public WeaponDatabase weaponData;
    public GameObject weaponChoose;
    public Bullet bullet1;
    private int indexWeapon;
    public bool isPlayerDie = false;
    public int indexMaterial;
    public GameObject effectLevelUp;
    public MeshRenderer weaponRenderer;
    public MeshFilter weaponMeshFilter;
    public MeshRenderer bulletRenderer;
    public MeshFilter bulletMeshFilter;
    [Header("------------------Change Pants------------------")]
    public PantsDatabases pantsData;                    // Data quần 
    public SkinnedMeshRenderer pantsOdPlayer;           // Renderer của quần nhân vật

    [Header("------------------Change Hats------------------")]
    public HatDatabases hatsData;                       // Data mũ
    public Transform hatAnchor;                         //Vị trí gắn mũ

    [Header("------------------Change Protect------------------")]
    public ShieldDatabases shieldData;                  // Data khiên 
    public Transform shieldAnchor;                      // Vị trí gắn khiên

    [Header("------------------Change Clothes Player------------------")]
    public SkinDatabases skinData;                      // Data skin nhân vật
    public SkinnedMeshRenderer initialShadingOfPlayer;  // Renderer thân của nhân vật
    public Transform[] list_anchorsOfSkin;              // Danh sách anchor của skin
    public Transform wingAnchor;                        // Vị trí gắn cánh
    public Transform tailAnchor;                        // Vị trí gắn đuôi

    [Header("------------------Shooting Settings------------------")]
    private float fireRate = 0.5f;      // thời gian chờ giữa các lần bắn
    private float nextFireTime = 0f;

    [Header("------------------Loading UI------------------")]
    public GameObject weaponLoadOfMenu;       // Hiển thị thanh/tải vũ khí
    public GameObject menuLoad;               // Menu loading
    public float speedRotation;               // Tốc độ xoay vòng loading

    [Header("----------------Game Progress-----------------")]
    public int zombieAlive;         // Số zombie còn sống
    public GameObject winner;       // UI khi thắng
    public int coinOfPlayerCity;    // Số coin hiện tại của player

    [Header("------------------Player Status------------------")]
    public bool isOffPlayer = false;            // Player đã bị loại chưa
    private int indexFunctionBullet;            // Kiểu bắn của player (dùng PlayerPrefs để lưu)
    public int levelOfPlayerCity;               // Level của player

    [Header("------------------Protection ------------------")]
    public bool isProtectPlayer = false;        // Trạng thái có khiên bảo vệ không
    public GameObject circlePtotect;            // Vòng tròn bảo vệ
    public float timerProtectPlayer;            // Thời gian còn lại của bảo vệ
    public int countProtect;                    // Số lần nhặt khiên
    public TextMeshProUGUI textCount;           // UI hiển thị số khiên còn lại

    [Header("---------------Speed Buff----------------")]
    public TextMeshProUGUI textOfCountSpeed;    // UI hiển thị tốc độ buff
    public int countSpeed;                      // Số lần buff tốc độ

    [Header("---------------Circle Settings---------------")]
    public DrawCircle drawCircle;               // Vẽ vòng tròn phạm vi
    public bool isSetCircle;                    // Có đang set vòng tròn không
    public TextMeshProUGUI textCircleRange;     // UI hiển thị kích thước vòng
    public int sizeCircle;                      // Bán kính vòng tròn
    public float number;                               // Biến phụ trợ (nếu dùng nội bộ)


    public void Init(){
        coinMoney = PlayerPrefs.GetInt("coinMoney");
        // Nếu skin đang dùng KHÔNG phải Skin_2 → set skin bình thường
        if (DataManager.Ins.gameSave.idSkin != "Skin_2"){
            SetSkinOfPlayer();
        }
        // Nếu là Skin mặc định → set thêm khiên, quần, mũ
        else
        {
            SetShieldOfPlayer();
            SetPantOfPlayer();
            SetHatOfPlayer();
        }
        SetWeaponOfPlayer();
    }
    //void Update(){
    //    // Nếu player đã chết thì ngừng update
    //    if (isDead) return;
    //    PlayerMove();
    //    AttackTrigle();
    //    //Tăng level player
    //    UpLevel();
    //    SetWinner();
    //    // ================== Xử lý Khiên Bảo Vệ ==================
    //    if (circlePtotect.activeSelf){
    //        timerProtectPlayer += Time.deltaTime;
    //        // Sau 3s thì tắt khiên
    //        if (timerProtectPlayer >= 3){
    //            circlePtotect.SetActive(false);
    //            timerProtectPlayer = 0;
    //            countProtect -= 1;    // Giảm số lần bảo vệ còn lại
    //        }
    //        // Nếu không còn lượt bảo vệ thì tắt trạng thái
    //        if (countProtect <= 0)
    //            isProtectPlayer = false;
    //    }
    //    // ================== Xử lý Vũ khí xoay khi load game ==================
    //    if (weaponLoadOfMenu != null)
    //        WeaponRotateWhenStartGame();
    //}

    //Thay đổi vũ khí
    public void SetWeaponOfPlayer()
    {
        indexWeapon = PlayerPrefs.GetInt("SelectOption");     //Lấy index của vũ khí 
        indexMaterial = PlayerPrefs.GetInt("MaterialOfWeapon" + indexWeapon);       //Lấy index của loại vũ khí 
        MeshRenderer meshRenderer = weaponRenderer;
        MeshRenderer meshRendererOfButton = bulletRenderer;
        Material[] mats = meshRenderer.materials;
        Material[] matsOfButton = meshRendererOfButton.sharedMaterials;
        string idWeapon = DataManager.Ins.gameSave.idWeapon;                // Lấy ID vũ khí hiện tại từ gameSave
        for (int i = 0; i < weaponData.weapon.Count(); i++)
        {                // Duyệt danh sách vũ khí để tìm vũ khí khớp với idWeapon
            if (weaponData.weapon[i].index == idWeapon)
            {
                // Gán mesh của vũ khí cho player và bullet
                weaponMeshFilter.mesh = weaponData.weapon[i].meshWeapon;
                bulletMeshFilter.mesh = weaponData.weapon[i].meshWeapon;

                // Thay đổi materials của vũ khí và bullet
                for (int j = 0; j < weaponData.listOfMaterials[indexWeapon].materialOfHammer[indexMaterial].materials.Length; j++)
                {
                    mats[j] = weaponData.listOfMaterials[indexWeapon].materialOfHammer[indexMaterial].materials[j];
                    matsOfButton[j] = weaponData.listOfMaterials[indexWeapon].materialOfHammer[indexMaterial].materials[j];
                }

                // Cập nhật materials cho MeshRenderer
                meshRenderer.materials = mats;
                meshRendererOfButton.materials = matsOfButton;

                //Thay đổi loại vũ khí xoay cho bullet 
                if (weaponData.weapon[i].isRotate)
                    bullet1.SetRoration = true;
                else
                    bullet1.SetRoration = false;
                //Thay đổi loại vũ khí bommerang cho bullet 
                if (weaponData.weapon[i].isBomerang)
                    bullet1.SetBoomerang = true;
                else
                    bullet1.SetBoomerang = false;
            }
        }
    }

    //Thay đổi quần của nhân vật 
    public void SetPantOfPlayer(){
        string pantName = DataManager.Ins.gameSave.idPant; // Lấy ID quần đã lưu của người chơi
        // Kiểm tra xem pantName có hợp lệ không
        if (!string.IsNullOrEmpty(pantName)){
            // Duyệt danh sách tất cả quần trong pantsData
            for (int i = 0; i < pantsData.pants.Length; i++){
                // Nếu tìm thấy quần có index trùng với pantName
                if (pantsData.pants[i].index == pantName){
                    pantsOdPlayer.material = pantsData.pants[i].material; // Gán material cho player
                }
            }
        }
    }

    //Thay đổi mũ của nhân vật
    public void SetHatOfPlayer(){
        string hatName = DataManager.Ins.gameSave.idHat; // Lấy ID mũ đã lưu của người chơi

        // Kiểm tra xem hatName có hợp lệ không
        if (!string.IsNullOrEmpty(hatName)){
            // Xóa tất cả mũ cũ đang gắn trên hatAnchor
            foreach (Transform child in hatAnchor.transform){
                Destroy(child.gameObject); // Hủy từng mũ con
            }

            // Duyệt danh sách tất cả mũ trong hatsData
            for (int i = 0; i < hatsData.hats.Length; i++){
                // Nếu tìm thấy mũ có index trùng với hatName
                if (hatsData.hats[i].index == hatName){
                    // Sinh mũ mới và gắn vào hatAnchor
                    Instantiate(hatsData.hats[i].hatPrefab, hatAnchor.transform);
                }
            }
        }
    }

    //Thay đổi khiên của nhân vật
    public void SetShieldOfPlayer()
    {
        string shieldName = DataManager.Ins.gameSave.idShield; // Lấy ID khiên đã lưu của người chơi

        // Kiểm tra xem shieldName có hợp lệ không
        if (!string.IsNullOrEmpty(shieldName))
        {
            // Xóa tất cả khiên cũ đang gắn trên shieldAnchor
            foreach (Transform child in shieldAnchor.transform)
            {
                Destroy(child.gameObject); // Hủy từng khiên con
            }

            // Duyệt danh sách tất cả khiên trong shieldData
            for (int i = 0; i < shieldData.shields.Length; i++)
            {
                // Nếu tìm thấy khiên có index trùng với shieldName
                if (shieldData.shields[i].index == shieldName)
                {
                    // Sinh khiên mới và gắn vào shieldAnchor
                    Instantiate(shieldData.shields[i].shieldPrefab, shieldAnchor.transform);
                }
            }
        }
    }

    //Thay đổi skin của nhân vật 
    public void SetSkinOfPlayer()
    {
        string skinName = DataManager.Ins?.gameSave?.idSkin; // Lấy ID skin đã lưu của người chơi

        // Nếu skinName rỗng hoặc null thì thoát khỏi hàm
        if (string.IsNullOrEmpty(skinName)) return;

        // Xóa tất cả các item cũ trong list_anchorsOfSkin
        foreach (Transform anchor in list_anchorsOfSkin)
        {
            foreach (Transform child in anchor)
            {
                Destroy(child.gameObject); // Hủy từng child trong anchor
            }
        }

        // Duyệt danh sách skin để tìm skin có index trùng với skinName
        for (int i = 0; i < skinData.skin.Length; i++)
        {
            // Nếu tìm thấy skin đúng
            if (skinData.skin[i].index == skinName)
            {
                // Sinh các phần của skin (mũ, khiên, cánh, đuôi) vào các anchor tương ứng
                Instantiate(skinData.skin[i].hatOfSkin, hatAnchor.transform);
                Instantiate(skinData.skin[i].shieldOfSkin, shieldAnchor.transform);
                Instantiate(skinData.skin[i].wingOfSkin, wingAnchor.transform);
                Instantiate(skinData.skin[i].tailOfSkin, tailAnchor.transform);

                // Cập nhật material của player và quần theo skin
                initialShadingOfPlayer.material = skinData.skin[i].materialOfPlayer; // Player
                pantsOdPlayer.material = skinData.skin[i].materialOfPlayer;          // Pants
            }
        }
    }

    //Hàm di chuyển nhân vật
    public void PlayerMove()
    {
        if (isAttack)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= attackDuration)
            {
                isAttack = false;
                anim.SetBool("Attack", false);
                weaponOfPlayerCity.SetActive(true);
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
            directionOfPlayerCity.x = joystick.Horizontal;
            directionOfPlayerCity.z = joystick.Vertical;
            directionOfPlayerCity.y = 0;

            Vector3 moveDir = directionOfPlayerCity.normalized * moveSpeedOfPlayerCity * Time.deltaTime;

            // ✅ Dùng MovePosition thay vì Translate
            rb.MovePosition(rb.position + moveDir);

            anim.SetFloat("Speed", directionOfPlayerCity.sqrMagnitude);

            if (directionOfPlayerCity.sqrMagnitude > 0.01f)
            {
                Quaternion toRotation = Quaternion.LookRotation(directionOfPlayerCity, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
                anim.SetBool("Attack", false);
            }
        }
    }

        // Hàm phát hiện enemy trong phạm vi tấn công
        public void AttackTrigle()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radiusAttackOfPlayerCity);        // Tìm tất cả Collider trong bán kính attack của player
        Zombie firstEnemyDetected = null;      // Biến lưu enemy đầu tiên phát hiện

        // Duyệt qua tất cả Collider tìm được
        foreach (var hit in colliders)
        {
            // Nếu collider là Enemy
            if (hit.CompareTag("EnemyController"))
            {
                firstEnemyDetected = hit.GetComponent<Zombie>(); // Lấy component Enemy từ collider
                break; // chỉ lấy enemy đầu tiên tìm thấy
            }
        }

        // Nếu có enemy trong phạm vi
        if (firstEnemyDetected != null)
        {
            // Nếu trước đó đã có enemyCurrent và enemy này KHÁC enemy vừa tìm thấy
            if (enemyCurrent != null && enemyCurrent != firstEnemyDetected)
            {
                enemyCurrent.circleTargetZombie.SetActive(false); // Tắt vòng tròn dưới chân
            }
            enemyCurrent = firstEnemyDetected;                // Cập nhật lại enemy hiện tại
            enemyCurrent.circleTargetZombie.SetActive(true); // Bật vòng tròn dưới chân

            // Nếu player ĐANG ĐỨNG YÊN (không di chuyển)
            if (directionOfPlayerCity.sqrMagnitude == 0.0f)
            {
                target = enemyCurrent.transform;          // Set target cho player
                anim.SetBool("Attack", true);             // Bật animation Attack

                // Quay mặt player về phía enemy
                Vector3 directionEnemy = target.position - transform.position;
                directionEnemy.y = 0; // không thay đổi trục Y
                Quaternion toRotation = Quaternion.LookRotation(directionEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }
            else
            {
                // Nếu player đang di chuyển thì không cho attack
                anim.SetBool("Attack", false);
            }
        }
        else
        {
            // Nếu không còn enemy nào trong phạm vi
            if (enemyCurrent != null)
            {
                enemyCurrent.circleTargetZombie.SetActive(false); // Tắt highlight enemy trước đó
                enemyCurrent = null; // Reset
            }
        }
    }

    public void Shooting()
    {
        AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Attack);
        // Lấy chế độ bắn từ PlayerPrefs (Function lưu trong bộ nhớ)
        indexFunctionBullet = PlayerPrefs.GetInt("Function");

        // Nếu functionBullet = -1 → dùng chế độ mặc định theo level
        if (indexFunctionBullet == -1)
        {
            // Level 0 → bắn 1 viên bình thường
            if (levelOfPlayerCity == 0)
            {
                ShootingDefault();
            }
            // Level 1 → bắn 2 viên song song
            else if (levelOfPlayerCity == 1)
            {
                ShootDualParallel(0.6f); // khoảng cách giữa 2 viên = 0.3
            }
        }
        // Nếu functionBullet = 0 → bắn 2 viên, 1 viên thẳng và 1 viên lệch góc
        else if (indexFunctionBullet == 0)
        {
            ShootDoubleSpread(45); // góc lệch 45 độ
        }
        // Nếu functionBullet = 1 → bắn liên tiếp 2 viên
        else if (indexFunctionBullet == 1)
        {
            // Kiểm tra cooldown trước khi bắn
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + fireRate; // set lại thời gian bắn tiếp theo
                StartCoroutine(ShootDoubleShot());  // bắn 2 viên liên tiếp (delay nhỏ giữa 2 viên)
            }
        }
        // Nếu functionBullet = 2 → bắn 3 viên tỏa
        else if (indexFunctionBullet == 2)
        {
            ShootTripleSpread();
        }
    }

    //Chế độ bắn mặc định
    public void ShootingDefault()
    {
        directionOfPlayerCity = Vector3.zero; // Reset hướng di chuyển của player (đứng yên khi bắn)


        GameObject bulletObj = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.identity);  // Tạo viên đạn mới từ prefab tại vị trí firingTransform

        Bullet bulletScript = bulletObj.GetComponent<Bullet>(); // Lấy script Bullet từ viên đạn

        bulletScript.SetOwner(gameObject); // Thiết lập owner cho viên đạn là player

        // if: nếu có target thì gán target cho đạn, tránh lỗi khi target null
        if (target != null)
            bulletScript.SetTarget(target);
    }

    //Bắn liên tiếp 2 viên 
    public IEnumerator ShootDoubleShot()
    {
        Vector3 saveTranform = firingTransform.position; // Lưu lại vị trí bắn ban đầu
        directionOfPlayerCity = Vector3.zero; // Dừng di chuyển của player khi bắn

        // if: nếu không có target thì thoát coroutine, không bắn
        if (target == null)
            yield break;

        Vector3 dir = (target.position - firingTransform.position).normalized; // Hướng bắn chuẩn theo target

        // Viên đạn thứ 1
        GameObject bulletObj1 = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.identity);
        Bullet b1 = bulletObj1.GetComponent<Bullet>();
        b1.SetOwner(gameObject);   // Set player làm chủ viên đạn
        b1.SetDirection(dir);      // Set hướng bay cho viên đạn

        yield return new WaitForSeconds(0.15f); // Delay 0.15s trước khi bắn viên thứ 2

        // Viên đạn thứ 2
        GameObject bulletObj2 = Instantiate(bulletPrefabs, saveTranform, Quaternion.identity);
        Bullet b2 = bulletObj2.GetComponent<Bullet>();
        b2.SetOwner(gameObject);   // Set player làm chủ viên đạn
        b2.SetDirection(dir);      // Set hướng bay giống viên thứ 1 (thẳng hàng)
    }

    //Bắn 2 viên song song 
    public void ShootDualParallel(float offsetDistance)
    {
        // if: nếu không có target thì không bắn
        if (target == null) return;

        directionOfPlayerCity = Vector3.zero; // Dừng di chuyển player khi bắn

        // Hướng bắn (vector từ player tới enemy, chuẩn hóa để làm hướng bay của đạn)
        Vector3 dirToTarget = (target.position - firingTransform.position).normalized;

        // Tính vector vuông góc với hướng bắn (dùng cross product với Vector3.up)
        // => cho ra hướng trái/phải để đặt 2 viên đạn song song
        Vector3 sideOffset = Vector3.Cross(Vector3.up, dirToTarget).normalized * offsetDistance;

        // Viên đạn 1 (dịch sang trái một khoảng offsetDistance)
        GameObject bulletLeft = Instantiate(
            bulletPrefabs,
            firingTransform.position - sideOffset, // vị trí dịch sang trái
            Quaternion.LookRotation(dirToTarget)   // xoay cùng hướng bắn
        );
        Bullet bLeft = bulletLeft.GetComponent<Bullet>();
        bLeft.SetOwner(gameObject);      // Set player làm chủ viên đạn
        bLeft.SetDirection(dirToTarget); // Đặt hướng bay của đạn

        // Viên đạn 2 (dịch sang phải một khoảng offsetDistance)
        GameObject bulletRight = Instantiate(
            bulletPrefabs,
            firingTransform.position + sideOffset, // vị trí dịch sang phải
            Quaternion.LookRotation(dirToTarget)   // xoay cùng hướng bắn
        );
        Bullet bRight = bulletRight.GetComponent<Bullet>();
        bRight.SetOwner(gameObject);      // Set player làm chủ viên đạn
        bRight.SetDirection(dirToTarget); // Đặt hướng bay của đạn
    }

    // Bắn 2 viên tách nhau
    public void ShootDoubleSpread(float angle)
    {
        if (target == null) return; // Nếu không có enemy thì không bắn

        directionOfPlayerCity = Vector3.zero; // Dừng di chuyển khi bắn

        // Hướng chuẩn đến enemy (viên đạn chính đi thẳng)
        Vector3 dirToTarget = (target.position - firingTransform.position).normalized;

        // === Viên đạn số 1: luôn đi thẳng về phía enemy ===
        GameObject bulletMain = Instantiate(
            bulletPrefabs,
            firingTransform.position,
            Quaternion.LookRotation(dirToTarget)
        );
        Bullet bMain = bulletMain.GetComponent<Bullet>();
        bMain.SetOwner(gameObject);
        bMain.SetDirection(dirToTarget);

        // === Viên đạn số 2: bay lệch sang một góc angle ===
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up); // Xoay quanh trục Y (lên/xuống)
        Vector3 sideDir = rot * dirToTarget; // Lấy hướng mới sau khi xoay

        GameObject bulletSide = Instantiate(
            bulletPrefabs,
            firingTransform.position,
            Quaternion.LookRotation(sideDir)
        );
        Bullet bSide = bulletSide.GetComponent<Bullet>();
        bSide.SetOwner(gameObject);
        bSide.SetDirection(sideDir);
    }

    //Bắn 3 viên đạn tách nhau
    public void ShootTripleSpread()
    {
        if (target == null) return; // Nếu không có enemy thì không bắn

        directionOfPlayerCity = Vector3.zero; // Dừng di chuyển khi bắn

        // Hướng chuẩn tới enemy (dùng cho viên đạn giữa)
        Vector3 dirToTarget = (target.position - firingTransform.position).normalized;

        // === Viên giữa: bắn thẳng vào enemy ===
        GameObject bulletMid = Instantiate(
            bulletPrefabs,
            firingTransform.position,
            Quaternion.LookRotation(dirToTarget)
        );
        Bullet bMid = bulletMid.GetComponent<Bullet>();
        bMid.SetOwner(gameObject);
        bMid.SetDirection(dirToTarget);

        // === Viên trái: bắn lệch góc -60 độ quanh trục Y ===
        Vector3 leftDir = Quaternion.AngleAxis(-60f, Vector3.up) * dirToTarget;
        GameObject bulletLeft = Instantiate(
            bulletPrefabs,
            firingTransform.position,
            Quaternion.LookRotation(leftDir)
        );
        Bullet bLeft = bulletLeft.GetComponent<Bullet>();
        bLeft.SetOwner(gameObject);
        bLeft.SetDirection(leftDir);

        // === Viên phải: bắn lệch góc +60 độ quanh trục Y ===
        Vector3 rightDir = Quaternion.AngleAxis(60f, Vector3.up) * dirToTarget;
        GameObject bulletRight = Instantiate(
            bulletPrefabs,
            firingTransform.position,
            Quaternion.LookRotation(rightDir)
        );
        Bullet bRight = bulletRight.GetComponent<Bullet>();
        bRight.SetOwner(gameObject);
        bRight.SetDirection(rightDir);
    }

    //Nâng cấp player
    public void UpLevel()
    {
        // Nếu coin của người chơi đúng bằng 15 thì cho phép lên level
        if (pointOfPlayerCity == 8)
        {
            // Kiểm tra biến textOfAnimLevelUp có tồn tại (điều kiện này bị viết 2 lần giống nhau)
            if (textOfAnimLevelUp != null || textOfAnimLevelUp != null)
            {
                AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Level_Up);
                levelOfPlayerCity = 1; // Gán level của người chơi = 1
                textOfAnimLevelUp.SetActive(true); // Hiển thị text "Level Up" trên UI
                animTextOfLevelUp.SetTrigger("Text_Move"); // Kích hoạt animation "Text_Move" để chạy hiệu ứng
            }
        }

    }
    public void SetWinner()
    {
        // Nếu số lượng enemy còn sống <= 0 thì kết thúc game
        if (ZombieCityController.instance.zombieTotal <= 0)
        {
            AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Win);
            ZombieCityController.instance.zombieTotal = 0; // Đảm bảo giá trị không xuống âm
            //winner.SetActive(true); // Hiển thị popUp thông báo người chơi chiến thắng

            //StartCoroutine(StopGameAfterDelay(1.5f));
        }
    }
    private IEnumerator StopGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Time.timeScale = 0f;
    }

    //Bảo vệ người chơi khỏi enemy 
    public void FunctionProtectPlayer()
    {
        countProtect += 1;
        textCount.text = countProtect.ToString();
        isProtectPlayer = true;
    }

    //Tăng tốc độ player
    public void FunctionUpVelocity()
    {
        countSpeed += 10;
        textOfCountSpeed.text = countSpeed.ToString();
        moveSpeedOfPlayerCity = moveSpeedOfPlayerCity + (moveSpeedOfPlayerCity * 0.2f);
    }

    // Tăng phạm vi tấn công 
    public void FunctionSetRangeAttack()
    {
        sizeCircle += 10;
        drawCircle.radius = 6.0f;
        radiusAttackOfPlayerCity = 6.0f;
        isSetCircle = true;
        textCircleRange.text = sizeCircle.ToString();
    }

    // Quay vũ khí khi vào game
    public void WeaponRotateWhenStartGame()
    {
        weaponLoadOfMenu.transform.rotation = Quaternion.Euler(0, 0, Time.time * -speedRotation);
        number -= Time.deltaTime;
        if (number < 0)
            menuLoad.SetActive(false);
    }
    public void SetOffAttack() => anim.SetBool("Attack", false);
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusAttackOfPlayerCity);
    }
    public void DestroyPlayer() => gameObject.SetActive(false);
    private void OnCollisionEnter(Collision collision)
    {
        // --- Nếu va chạm với đạn của enemy (Bullet2) ---
        if (collision.gameObject.CompareTag("Bullet2"))
        {

            dead1.SetActive(true);                     // Hiển thị hiệu ứng UI chết
            anim.SetBool("Death", true);               // Chạy animation chết
            weaponOfPlayerCity.SetActive(false);       // Ẩn vũ khí của player
            isDead = true;                             // Đánh dấu player đã chết
            PlayerPrefs.SetInt("coinMoney", coinMoney);// Lưu lại số coin hiện tại vào PlayerPrefs
            isPlayerDie = true;                        // Cờ kiểm tra player đã chết
        }

        // --- Nếu va chạm với enemy trực tiếp (EnemyController) ---
        if (collision.gameObject.CompareTag("EnemyController"))
        {
            // Nếu đang bật khiên bảo vệ
            if (isProtectPlayer)
            {
                circlePtotect.SetActive(true);          // Hiển thị vòng bảo vệ
            }
            // Nếu không có bảo vệ → chết
            else
            {
                AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Lose);
                //dead1.SetActive(true);                  // Hiển thị UI chết
                isDead = true;                          // Đánh dấu player đã chết
                anim.SetBool("Death", true);            // Chạy animation chết
                isOffPlayer = true;                     // Đánh dấu player đã bị loại
            }
        }
    }
}
