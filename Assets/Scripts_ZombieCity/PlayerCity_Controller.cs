using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCity_Controller : MonoBehaviour
{
    public Animator anim;
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
    public WeaponDatabase weaponDB;
    public Test test;
    public GameObject weaponChoose;
    public Bullet bullet1;
    private int indexWeapon;
    public bool isPlayerDie = false;
    [SerializeField] private int indexMaterial;

    public int countAttack;
    public GameObject effectLevelUp;
    [Header("Change Pants")]
    [SerializeField] private int indexPants;
    [SerializeField] private ListPants listPants;
    [SerializeField] private GameObject pantsOdPlayer;

    [Header("Change Hats")]
    [SerializeField] private int indexHats;
    [SerializeField] private HATS hatOfPlayer;

    [Header("Change Protect")]
    [SerializeField] private int indexProtect;
    [SerializeField] private Protect protectOfPlayer;

    [Header("Change Clothes Player")]
    [SerializeField] private int indexClothes;
    [SerializeField] private ClothesSet[] listClothes;
    [SerializeField] private GameObject initialShadingOfPlayer;
    [SerializeField] private GameObject PantsOfPlayer;

    [Header("Attack Settings")]
    [SerializeField] private float fireRate = 0.5f; // thời gian chờ giữa các lần bắn
    private float nextFireTime = 0f;

    public int EnemyAlive;
    public GameObject winner;
    public int coinOfPlayer;

    public bool isOffPlayer = false;
    private int functionBullet;
    //[SerializeField] private Bullet bullet1;
    void Start()
    {
        coinOfPlayer = 0;
        EnemyAlive = 10;
        point = 0;
        coinMoney = PlayerPrefs.GetInt("coinMoney");
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (isDead) return;
        PlayerMove();
        AttackTrigle();
        //Thay doi quan ao, vu khi
        changeWepon();
        changePants();
        changeHats();
        changeProtect();
        changeClothesPlayer();
        //Len level
        UpLevel();
        SetWinner();
    }
    void changeWepon()
    {
        indexWeapon = PlayerPrefs.GetInt("IndexWeapon");
        indexMaterial = PlayerPrefs.GetInt("MaterialOfWeapon" + indexWeapon);
        MeshRenderer meshRenderer = weaponChoose.GetComponent<MeshRenderer>();
        MeshRenderer meshRendererOfButton = bullet1.GetComponent<MeshRenderer>();
        // Lấy toàn bộ materials ra
        Material[] mats = meshRenderer.materials;
        Material[] matsOfButton = meshRendererOfButton.sharedMaterials;
        for (int i = 0; i < test.list.Count(); i++)
        {
            if (test.list[i].index == indexWeapon)
            {
                weaponChoose.GetComponent<MeshFilter>().mesh = test.list[i].weaponMesh;
                bullet1.GetComponent<MeshFilter>().mesh = test.list[i].weaponMesh;
                for (int j = 0; j < weaponDB.listOfMaterials[indexWeapon].materialOfHammer[indexMaterial].materials.Length; j++)
                {
                    mats[j] = weaponDB.listOfMaterials[indexWeapon].materialOfHammer[indexMaterial].materials[j];
                    matsOfButton[j] = weaponDB.listOfMaterials[indexWeapon].materialOfHammer[indexMaterial].materials[j];
                }
                meshRenderer.materials = mats;
                meshRendererOfButton.materials = matsOfButton;
                if (test.list[i].isRotate)
                {
                    bullet1.SetRoration = true;
                }
                else
                {
                    bullet1.SetRoration = false;
                }
            }
        }
    }
    void changePants()
    {

        indexHats = PlayerPrefs.GetInt("SlectPaint", -1);
        //indexPants = PlayerPrefs.GetInt("IndexPants");
        if (indexHats == -1)
        {
            pantsOdPlayer.GetComponent<SkinnedMeshRenderer>().material = listPants.pantsObjects[6].materialPants;
        }
        for (int i = 0; i < listPants.pantsObjects.Count(); i++)
        {
            if (listPants.pantsObjects[i].index == indexHats)
            {
                pantsOdPlayer.GetComponent<SkinnedMeshRenderer>().material = listPants.pantsObjects[i].materialPants;
            }
        }
    }
    void changeHats()
    {
        //indexHats = PlayerPrefs.GetInt("IndexHat");
        indexHats = PlayerPrefs.GetInt("SlectHat", -1);
        if (indexHats == -1)
        {
            hatOfPlayer.games[6].SetActive(true);
        }
        for (int i = 0; i < hatOfPlayer.games.Count(); i++)
        {
            if (indexHats == i)
            {
                hatOfPlayer.games[i].SetActive(true);
            }
            else
            {
                hatOfPlayer.games[i].SetActive(false);
            }
        }
    }
    void changeProtect()
    {
        indexProtect = PlayerPrefs.GetInt("SlectProtect", -1);
        if (indexProtect == -1)
        {
            protectOfPlayer.protect[2].SetActive(true);
        }
        for (int i = 0; i < protectOfPlayer.protect.Count(); i++)
        {
            if (indexProtect == i)
            {
                protectOfPlayer.protect[i].SetActive(true);
            }
            else
            {
                protectOfPlayer.protect[i].SetActive(false);
            }
        }
    }

    void changeClothesPlayer()
    {
        indexClothes = PlayerPrefs.GetInt("SlectClothes", -1);
        if (indexProtect == -1)
        {
            listClothes[2].hatOfSet.SetActive(true);
            listClothes[2].wingOfSet.SetActive(true);
            listClothes[2].protectOfSet.SetActive(true);
            listClothes[2].tailOfSet.SetActive(true);
            //initialShadingOfPlayer.GetComponent<SkinnedMeshRenderer>().material = listClothes[2].material;
            //PantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = listClothes[2].material;
            //return;
        }
        for (int i = 0; i < listClothes.Count(); i++)
        {
            if (indexClothes == i)
            {
                listClothes[i].hatOfSet.SetActive(true);
                listClothes[i].wingOfSet.SetActive(true);
                listClothes[i].protectOfSet.SetActive(true);
                listClothes[i].tailOfSet.SetActive(true);
                initialShadingOfPlayer.GetComponent<SkinnedMeshRenderer>().material = listClothes[i].material;
                PantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = listClothes[i].material;
            }
            else
            {
                listClothes[2].hatOfSet.SetActive(false);
                listClothes[2].wingOfSet.SetActive(false);
                listClothes[2].protectOfSet.SetActive(false);
                listClothes[2].tailOfSet.SetActive(false);
                //initialShadingOfPlayer.GetComponent<SkinnedMeshRenderer>().material = listClothes[2].material;
                //PantsOfPlayer.GetComponent<SkinnedMeshRenderer>().material = listClothes[2].material;
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
            ShootingDefault();
        }
        else if(functionBullet == 0)
        {
            ShootingDouble(25);
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
            ShootingTriple(20);
        }
    }
    public void ShootingDefault()
    {
        playerMove = Vector3.zero;
        AudioManager.instance.PlayerSFX(0);
        GameObject bulletObj = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        bulletScript.SetOwner(gameObject);
        bulletScript.SetTarget(target);
    }
    private IEnumerator ShootLevel1()
    {
        Vector3 saveTranform = firingTransform.position;
        // Phát âm thanh bắn
        AudioManager.instance.PlayerSFX(0);

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


    public void ShootingDouble(float angle)
    {
        if (target == null) return;

        playerMove = Vector3.zero;
        AudioManager.instance.PlayerSFX(0);

        Vector3 dirToTarget = (target.position - firingTransform.position).normalized;

        // Góc lệch sang trái
        Vector3 leftDir = Quaternion.Euler(0, -angle, 0) * dirToTarget;
        GameObject bulletLeft = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.LookRotation(leftDir));
        Bullet bulletLeftScript = bulletLeft.GetComponent<Bullet>();
        bulletLeftScript.SetOwner(gameObject);
        bulletLeftScript.SetDirection(leftDir);

        // Góc lệch sang phải
        Vector3 rightDir = Quaternion.Euler(0, angle, 0) * dirToTarget;
        GameObject bulletRight = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.LookRotation(rightDir));
        Bullet bulletRightScript = bulletRight.GetComponent<Bullet>();
        bulletRightScript.SetOwner(gameObject);
        bulletRightScript.SetDirection(rightDir);
    }

    public void ShootingTriple(float angle)
    {
        if (target == null) return;

        playerMove = Vector3.zero;
        AudioManager.instance.PlayerSFX(0);

        Vector3 dirToTarget = (target.position - firingTransform.position).normalized;

        // Góc lệch sang trái
        Vector3 leftDir = Quaternion.Euler(0, -angle, 0) * dirToTarget;
        GameObject bulletLeft = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.LookRotation(leftDir));
        Bullet bulletLeftScript = bulletLeft.GetComponent<Bullet>();
        bulletLeftScript.SetOwner(gameObject);
        bulletLeftScript.SetDirection(leftDir);

        // Góc lệch sang phải
        Vector3 rightDir = Quaternion.Euler(0, angle, 0) * dirToTarget;
        GameObject bulletRight = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.LookRotation(rightDir));
        Bullet bulletRightScript = bulletRight.GetComponent<Bullet>();
        bulletRightScript.SetOwner(gameObject);
        bulletRightScript.SetDirection(rightDir);

        //Lệch giữa
        Vector3 betweenDir = Quaternion.Euler(0, 0, 0) * dirToTarget;
        GameObject bulletbetween = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.LookRotation(betweenDir));
        Bullet bulletBetweenScript = bulletbetween.GetComponent<Bullet>();
        bulletBetweenScript.SetOwner(gameObject);
        bulletBetweenScript.SetDirection(betweenDir);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet2"))
        {
            //AudioManager.instance.isPlayerBGM = false;
            //AudioManager.instance.PlayerSFX(0);
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
            //AudioManager.instance.PlayerSFX(3);
            dead1.SetActive(true);
            UIManager.instance.StartDead();
            isDead = true;
            anim.SetBool("Death", true);
            isOffPlayer = true;
        }
    }
    public void DestroyPlayer()
    {
        gameObject.SetActive(false);
    }
    public void UpLevel()
    {
        if (countAttack >= 1)
        {
            effectLevelUp.SetActive(true);
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            UIManager.instance.up = 4.5f;
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
}
