using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerCity_Controller : MonoBehaviour
{
    public Animator anim;
    public Animator animText;
    public GameObject textAnim;
    public Joystick joystick;

    [Header("Bullet")]
    [SerializeField] private GameObject bulletPrefabs;
    [SerializeField] private Transform firingTransform;

    [Header("Move Info")]
    [SerializeField] private float moveSpeed;
    private Vector3 playerMove;

    [Header("Radius")]
    [SerializeField] private float radius;

    [SerializeField] private GameObject Harmmer;
    [SerializeField] private Transform target;
    private bool isAttack = false;
    float timer;
    [SerializeField] private float attackDuration = 1f; // thời gian duy trì trạng thái attack
    private float attackTimer = 0f;
    public int point;
    private EnemyController enemyCurrent;
    private bool isDetech = false;
    public bool isDead = false;

    public GameObject dead1;
    public int coinMoney;

    [Header("Change Weapon")]
    public WeaponDatabase weaponData;
    public Test test;
    public GameObject weaponChoose;
    public Bullet bullet1;
    private int indexWeapon;
    public bool isPlayerDie = false;
    [SerializeField] private int indexMaterial;

    public int countAttack;
    public GameObject effectLevelUp;
    [Header("------------------Change Pants------------------")]
    public PantsDatabases pantsData;                 // Data quần 
    public SkinnedMeshRenderer pantsOdPlayer;        // Renderer của quần nhân vật

    [Header("------------------Change Hats------------------")]
    public HatDatabases hatsData;                   // Data mũ
    public Transform hatAnchor;                     //Vị trí gắn mũ

    [Header("------------------Change Protect------------------")]
    public ShieldDatabases shieldData;              // Data khiên 
    public Transform shieldAnchor;                  // Vị trí gắn khiên

    [Header("------------------Change Clothes Player------------------")]
    public SkinDatabases skinData;                          // Data skin nhân vật
    public SkinnedMeshRenderer initialShadingOfPlayer;      // Renderer thân của nhân vật
    public Transform[] list_anchorsOfSkin;                  // Danh sách anchor của skin
    public Transform wingAnchor;                            // Vị trí gắn cánh
    public Transform tailAnchor;                            // Vị trí gắn đuôi

    [Header("Attack Settings")]
    [SerializeField] private float fireRate = 0.5f; // thời gian chờ giữa các lần bắn
    private float nextFireTime = 0f;


    public GameObject weaponLoad;
    public GameObject menuLoad;
    [SerializeField] public float speedRotation;

    public int EnemyAlive;
    public GameObject winner;
    public int coinOfPlayer;

    public bool isOffPlayer = false;
    private int functionBullet;
    public int level;

    //Func 1
    public bool isProtectPlayer = false;
    public GameObject circlePtotect;
    public float timerProtectPlayer;
    public int countProtect;
    public TextMeshProUGUI textCount;
    //Func2
    public TextMeshProUGUI textSpeed;
    public int countSpeed;

    //Func3
    public DrawCircle drawCircle;
    public bool isSetCircle;
    public TextMeshProUGUI textCircleRange;
    public int sizeCircle;
    float number;


    void Start()
    {
        number = 2f;
        coinOfPlayer = 0;
        EnemyAlive = 25;
        point = 0;
        coinMoney = PlayerPrefs.GetInt("coinMoney");
        anim = GetComponent<Animator>();

        if (DataManager.Ins.gameSave.idSkin != "Skin_2")
        {
            SetSkinOfPlayer();
        }
        else
        {
            SetShieldOfPlayer();
            SetPantOfPlayer();
            SetHatOfPlayer();
        }
        SetWeaponOfPlayer();
    }
    void Update()
    {
        if (isDead) return;
        PlayerMove();
        AttackTrigle();
        //Thay doi quan ao, vu khi

        //Len level
        UpLevel();
        SetWinner();
        if (circlePtotect.activeSelf)
        {
            timerProtectPlayer += Time.deltaTime;
            if(timerProtectPlayer >= 3)
            {
                circlePtotect.SetActive(false);
                timerProtectPlayer = 0;
                countProtect -= 1;
            }
            if(countProtect <= 0)
            {
                isProtectPlayer = false;
            }
        }
        if (weaponLoad != null)
        {
            RorateWeapon();
        }
    }
    void SetWeaponOfPlayer()
    {
        indexWeapon = PlayerPrefs.GetInt("SelectOption");
        indexMaterial = PlayerPrefs.GetInt("MaterialOfWeapon" + indexWeapon);
        MeshRenderer meshRenderer = weaponChoose.GetComponent<MeshRenderer>();
        MeshRenderer meshRendererOfButton = bullet1.GetComponent<MeshRenderer>();
        Material[] mats = meshRenderer.materials;
        Material[] matsOfButton = meshRendererOfButton.sharedMaterials;
        string idWeapon = DataManager.Ins.gameSave.idWeapon;
        for (int i = 0; i < weaponData.weapon.Count(); i++)
        {
            if (weaponData.weapon[i].index == idWeapon)
            {
                weaponChoose.GetComponent<MeshFilter>().mesh = weaponData.weapon[i].meshWeapon;
                bullet1.GetComponent<MeshFilter>().mesh = weaponData.weapon[i].meshWeapon;
                for (int j = 0; j < weaponData.listOfMaterials[indexWeapon].materialOfHammer[indexMaterial].materials.Length; j++)
                {
                    mats[j] = weaponData.listOfMaterials[indexWeapon].materialOfHammer[indexMaterial].materials[j];
                    matsOfButton[j] = weaponData.listOfMaterials[indexWeapon].materialOfHammer[indexMaterial].materials[j];
                }
                meshRenderer.materials = mats;
                meshRendererOfButton.materials = matsOfButton;

                if (weaponData.weapon[i].isRotate)
                {
                    bullet1.SetRoration = true;
                }
                else
                {
                    bullet1.SetRoration = false;
                }

                if (weaponData.weapon[i].isBomerang)
                {
                    bullet1.SetBoomerang = true;
                }
                else
                {
                    bullet1.SetBoomerang = false;
                }
            }
        }
    }
    void SetPantOfPlayer()
    {
        string pantName = DataManager.Ins.gameSave.idPant;
        if (!string.IsNullOrEmpty(pantName))
        {
            for (int i = 0; i < pantsData.pants.Length; i++)
            {
                if (pantsData.pants[i].index == pantName)
                {
                    pantsOdPlayer.material = pantsData.pants[i].material;
                }
            }
        }
    }
    void SetHatOfPlayer()
    {
        string hatName = DataManager.Ins.gameSave.idHat;
        if (!string.IsNullOrEmpty(hatName))
        {
            foreach (Transform child in hatAnchor.transform)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < hatsData.hats.Length; i++)
            {
                if (hatsData.hats[i].index == hatName)
                {
                    Instantiate(hatsData.hats[i].hatPrefab, hatAnchor.transform);
                }
            }
        }
    }
    void SetShieldOfPlayer()
    {
        string shieldName = DataManager.Ins.gameSave.idShield;
        if (!string.IsNullOrEmpty(shieldName))
        {
            foreach (Transform child in shieldAnchor.transform)
            {
                Destroy(child.gameObject);
            }
            for (int i = 0; i < shieldData.shields.Length; i++)
            {
                if (shieldData.shields[i].index == shieldName)
                {
                    Instantiate(shieldData.shields[i].shieldPrefab, shieldAnchor.transform);
                }
            }
        }
    }

    void SetSkinOfPlayer()
    {
        string skinName = DataManager.Ins?.gameSave?.idSkin;
        if (string.IsNullOrEmpty(skinName)) return;

        foreach (Transform anchor in list_anchorsOfSkin)
        {
            foreach (Transform child in anchor)
            {
                Destroy(child.gameObject);
            }
        }
        for (int i = 0; i < skinData.skin.Length; i++)
        {
            if (skinData.skin[i].index == skinName)
            {
                Instantiate(skinData.skin[i].hatOfSkin, hatAnchor.transform);
                Instantiate(skinData.skin[i].shieldOfSkin, shieldAnchor.transform);
                Instantiate(skinData.skin[i].wingOfSkin, wingAnchor.transform);
                Instantiate(skinData.skin[i].tailOfSkin, tailAnchor.transform);
                initialShadingOfPlayer.material = skinData.skin[i].materialOfPlayer;
                pantsOdPlayer.material = skinData.skin[i].materialOfPlayer;
            }
        }
    }
    private void PlayerMove()
    {
        if (isAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                isAttack = false;
                anim.SetBool("Attack", false);
                Harmmer.SetActive(true);
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
            playerMove.x = joystick.Horizontal;
            playerMove.z = joystick.Vertical;
            playerMove.y = 0;

            Vector3 movement = playerMove * moveSpeed * Time.deltaTime;
            transform.Translate(movement, Space.World);
            anim.SetFloat("Speed", playerMove.sqrMagnitude);

            if (playerMove.sqrMagnitude > 0.01f)
            {
                Quaternion toRotation = Quaternion.LookRotation(playerMove, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    public void AttackTrigle()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        EnemyController firstEnemyDetected = null;

        foreach (var hit in colliders)
        {
            if (hit.CompareTag("EnemyController"))
            {
                firstEnemyDetected = hit.GetComponent<EnemyController>();
                break; // chỉ lấy enemy đầu tiên
            }
        }

        if (firstEnemyDetected != null)
        {
            if (enemyCurrent != null && enemyCurrent != firstEnemyDetected)
            {
                enemyCurrent.targetEnemy.SetActive(false); // Tắt enemy cũ nếu khác
            }

            enemyCurrent = firstEnemyDetected;
            enemyCurrent.targetEnemy.SetActive(true); // Bật enemy mới
            if (playerMove.sqrMagnitude == 0.0f)
            {
                target = enemyCurrent.transform;
                anim.SetBool("Attack", true);
                Vector3 directionEnemy = target.position - transform.position;
                directionEnemy.y = 0;
                Quaternion toRotation = Quaternion.LookRotation(directionEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }
            else
            {
                anim.SetBool("Attack", false);
            }
        }
        else
        {
            if (enemyCurrent != null)
            {
                enemyCurrent.targetEnemy.SetActive(false);
                enemyCurrent = null;
            }
        }
    }

    public void SetOffAttack() => anim.SetBool("Attack", false);
    public void Shooting()
    {
        functionBullet = PlayerPrefs.GetInt("Function");
        if (functionBullet == -1)
        {
            if(level == 0)
            {
                ShootingDefault();
            }
            else if(level == 1)
            {
                ShootingLevelUp(0.3f);
            }
        }
        else if(functionBullet == 0)
        {
            ShootingDouble(45);
        }
        else if(functionBullet == 1)
        {
            if (Time.time >= nextFireTime)
            {
            nextFireTime = Time.time + fireRate;
            StartCoroutine(ShootLevel1());
            }
        }
        else if (functionBullet == 2)
        {
            ShootingTriple();
        }
    }
    public void ShootingDefault()
    {
        playerMove = Vector3.zero;
        GameObject bulletObj = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetOwner(gameObject);
        bulletScript.SetTarget(target);
    }
    private IEnumerator ShootLevel1()
    {
        Vector3 saveTranform = firingTransform.position;

        // Dừng di chuyển của player khi bắn
        playerMove = Vector3.zero;

        if (target == null)
            yield break;

        // Lấy hướng bắn 1 lần duy nhất để 2 viên thẳng hàng
        Vector3 dir = (target.position - firingTransform.position).normalized;

        // Viên thứ 1
        GameObject bulletObj1 = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.identity);
        Bullet b1 = bulletObj1.GetComponent<Bullet>();
        b1.SetOwner(gameObject);
        b1.SetDirection(dir);

        // Delay nếu muốn bắn liên tiếp (0.05s)
        yield return new WaitForSeconds(0.15f);

        // Viên thứ 2
        GameObject bulletObj2 = Instantiate(bulletPrefabs, saveTranform, Quaternion.identity);
        Bullet b2 = bulletObj2.GetComponent<Bullet>();
        b2.SetOwner(gameObject);
        b2.SetDirection(dir);
    }
    public void ShootingLevelUp (float offsetDistance)
    {
        if (target == null) return;

        playerMove = Vector3.zero;

        // Hướng bắn (hướng tới enemy, nhưng cả 2 viên đều đi song song theo hướng này)
        Vector3 dirToTarget = (target.position - firingTransform.position).normalized;

        // Tính vector vuông góc để dịch ngang (dùng cross với Vector3.up để lấy hướng trái/phải)
        Vector3 sideOffset = Vector3.Cross(Vector3.up, dirToTarget).normalized * offsetDistance;

        // Viên đạn 1 (dịch sang trái)
        GameObject bulletLeft = Instantiate(
            bulletPrefabs,
            firingTransform.position - sideOffset,
            Quaternion.LookRotation(dirToTarget)
        );
        Bullet bLeft = bulletLeft.GetComponent<Bullet>();
        bLeft.SetOwner(gameObject);
        bLeft.SetDirection(dirToTarget);

        // Viên đạn 2 (dịch sang phải)
        GameObject bulletRight = Instantiate(
            bulletPrefabs,
            firingTransform.position + sideOffset,
            Quaternion.LookRotation(dirToTarget)
        );
        Bullet bRight = bulletRight.GetComponent<Bullet>();
        bRight.SetOwner(gameObject);
        bRight.SetDirection(dirToTarget);
    }
    public void ShootingDouble(float angle)
    {
        if (target == null) return;

        playerMove = Vector3.zero;

        // Hướng chuẩn đến enemy
        Vector3 dirToTarget = (target.position - firingTransform.position).normalized;

        // === Viên đạn số 1: luôn trúng enemy ===
        GameObject bulletMain = Instantiate(
            bulletPrefabs,
            firingTransform.position,
            Quaternion.LookRotation(dirToTarget)
        );
        Bullet bMain = bulletMain.GetComponent<Bullet>();
        bMain.SetOwner(gameObject);
        bMain.SetDirection(dirToTarget);

        // === Viên đạn số 2: cùng vị trí spawn, lệch góc bay ===
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up); // xoay quanh trục thẳng đứng
        Vector3 sideDir = rot * dirToTarget;

        GameObject bulletSide = Instantiate(
            bulletPrefabs,
            firingTransform.position,
            Quaternion.LookRotation(sideDir)
        );
        Bullet bSide = bulletSide.GetComponent<Bullet>();
        bSide.SetOwner(gameObject);
        bSide.SetDirection(sideDir);
    }
    public void ShootingTriple()
    {
        if (target == null) return;

        playerMove = Vector3.zero;

        // Hướng chuẩn đến enemy
        Vector3 dirToTarget = (target.position - firingTransform.position).normalized;

        // === Viên giữa: luôn trúng enemy ===
        GameObject bulletMid = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.LookRotation(dirToTarget));
        Bullet bMid = bulletMid.GetComponent<Bullet>();
        bMid.SetOwner(gameObject);
        bMid.SetDirection(dirToTarget);

        // === Viên trái: lệch -90 độ quanh trục Y ===
        Vector3 leftDir = Quaternion.AngleAxis(-60f, Vector3.up) * dirToTarget;
        GameObject bulletLeft = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.LookRotation(leftDir));
        Bullet bLeft = bulletLeft.GetComponent<Bullet>();
        bLeft.SetOwner(gameObject);
        bLeft.SetDirection(leftDir);

        // === Viên phải: lệch +90 độ quanh trục Y ===
        Vector3 rightDir = Quaternion.AngleAxis(60f, Vector3.up) * dirToTarget;
        GameObject bulletRight = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.LookRotation(rightDir));
        Bullet bRight = bulletRight.GetComponent<Bullet>();
        bRight.SetOwner(gameObject);
        bRight.SetDirection(rightDir);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet2"))
        {
            dead1.SetActive(true);
            UIManager.instance.StartDead();
            anim.SetBool("Death", true);
            Harmmer.SetActive(false);
            isDead = true;
            PlayerPrefs.SetInt("coinMoney", coinMoney);
            isPlayerDie = true;
        }
        if (collision.gameObject.CompareTag("EnemyController"))
        {
            if (isProtectPlayer)
            {
                circlePtotect.SetActive(true);

            }
            else
            {
                dead1.SetActive(true);
                UIManager.instance.StartDead();
                isDead = true;
                anim.SetBool("Death", true);
                isOffPlayer = true;
            }
        }
    }
    public void DestroyPlayer()
    {
        gameObject.SetActive(false);
    }
    public void UpLevel()
    {
        if (coinOfPlayer == 15)
        {
            if(textAnim != null || animText != null)
            {
                level = 1;
                textAnim.SetActive(true);
                animText.SetTrigger("Text_Move");
            }
        }
    }
    public void SetWinner()
    {
        if(EnemyAlive <= 0)
        {
            EnemyAlive = 0;
            winner.SetActive(true);
            StartCoroutine(StopGameAfterDelay(2f));
        }
    }
    private IEnumerator StopGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Time.timeScale = 0f;
    }
    public void OnProtect()
    {
        countProtect += 1;
        textCount.text = countProtect.ToString();
        isProtectPlayer = true;
    }
    public void SetVelocity()
    {
        countSpeed += 10;
        textSpeed.text = countSpeed.ToString(); 
        moveSpeed = moveSpeed + (moveSpeed * 0.2f);
    }
    public void SetCircleRange()
    {
        sizeCircle += 10;
        drawCircle.radius = 6.0f;
        radius = 6.0f;
        isSetCircle = true;
        textCircleRange.text = sizeCircle.ToString();
    }

    public void RorateWeapon()
    {

        weaponLoad.transform.rotation = Quaternion.Euler(0, 0, Time.time * -speedRotation);
        number -= Time.deltaTime;
        if (number < 0)
        {
            menuLoad.SetActive(false);
        }
    }
}
