using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public Animator anim;        // Animator của nhân vật
    public Joystick joystick;    // Joystick điều khiển nhân vật

    [Header("------------------Bullet------------------")]
    public GameObject bulletPrefabs;   // Prefabs của viên đạn
    public Transform firingTransform;  // Vị trí viên đạn được bắn ra

    [Header("------------------Move Info------------------")]
    public float moveSpeedOfPlayer;           // Tốc độ di chuyển của nhân vật
    private Vector3 directionOfPlayer;        // Hướng di chuyển của nhân vật dựa trên joystick

    [Header("------------------Radius------------------")]
    public float radiusAttackOfPlayer;  //  kính vòng tròn phát hiện Enemy của Player

    public GameObject Harmmer;          // Vũ khí của nhân vật
    private Transform targetEnemy;      // Vị trí của Enemy
    public bool isAttack = false;       // Cho biết xem có tấn công hay không
    public float attackDuration = 1f;   // Thời gian duy trì trạng thái tấn công
    public int point;                   // Điểm của người chơi
    private Enemy enemyCurrent;                            
    public bool isDead = false;   // Kiểm tra xem nhân vật đã chết hay chưa
    public GameObject dead1;
    public int coinMoney;       // Tiền của người chơi

    [Header("------------------Change Weapon------------------")]
    public WeaponDatabase weaponDB;
    public Test test;
    public GameObject weaponChoose;
    public Bullet bullet1;              // Viên đạn của người chơi
    private int indexWeapon;
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


    public bool isGetGift = false;
    public bool isLevelUp = false;
    void Start()
    {
        point = 0;
        coinMoney = PlayerPrefs.GetInt("coinMoney");
        isGetGift = false;
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
    }
    void Update()
    {
        if (isDead) return;
        PlayerMove();
        AttackTrigle();
        UpLevel();
    }
    //Thay đổi vũ khí
     void SetWeaponOfPlayer()
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

    //Thay đổi quần của nhân vật 
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

    //Thay đổi mũ của nhân vật
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

    //Thay đổi khiên của nhân vật
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

    //Thay đổi skin của nhân vật 
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

    //Hàm di chuyển nhân vật
    private void PlayerMove(){
        // Lấy giá trị đầu vào từ joystick để xác định hướng di chuyển
        directionOfPlayer.x = joystick.Horizontal;   // Trục X
        directionOfPlayer.z = joystick.Vertical;     // Trục Z
        directionOfPlayer.y = 0;              // Không thay đổi chiều cao (Y = 0)
        transform.Translate(directionOfPlayer * moveSpeedOfPlayer * Time.deltaTime, Space.World);   // Di chuyển nhân vật theo hướng đã lấy được
        anim.SetFloat("Speed", directionOfPlayer.sqrMagnitude);    // Gửi giá trị để điều khiển animation
        
        // Nếu nhân vật thực sự có di chuyển (vector khác 0)
        if (directionOfPlayer.sqrMagnitude > 0.01f){    
            Quaternion toRotation = Quaternion.LookRotation(directionOfPlayer, Vector3.up);                 // Tạo hướng xoay dựa trên vector di chuyển
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);    // Xoay nhân vật mượt về hướng di chuyển
        }
    }

    // Hàm phát hiện enemy trong phạm vi tấn công
    public void AttackTrigle(){
        // Tạo một mảng colliders để chứa tất cả đối tượng trong bán kính phát hiện
        Collider[] colliders = Physics.OverlapSphere(transform.position, radiusAttackOfPlayer);
        Enemy firstEnemyDetected = null;
        foreach (var hit in colliders){
            if (hit.CompareTag("Enemy"))
            {
                firstEnemyDetected = hit.GetComponent<Enemy>();
                break; // chỉ lấy enemy đầu tiên
            }
        }
        if (firstEnemyDetected != null){
            if (enemyCurrent != null && enemyCurrent != firstEnemyDetected){
                enemyCurrent.targetEnemy.SetActive(false); // Tắt enemy cũ nếu khác
            }
            enemyCurrent = firstEnemyDetected;
            enemyCurrent.targetEnemy.SetActive(true); // Bật enemy mới
            if (directionOfPlayer.sqrMagnitude == 0.0f)
            {
                targetEnemy = enemyCurrent.transform;
                anim.SetBool("Attack", true);
                Vector3 directionEnemy = targetEnemy.position - transform.position;
                directionEnemy.y = 0;
                Quaternion toRotation = Quaternion.LookRotation(directionEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }
            else{
                anim.SetBool("Attack", false);
            }
        }
        else{
            if (enemyCurrent != null){
                enemyCurrent.targetEnemy.SetActive(false);
                enemyCurrent = null;
            }
        }
    }

    public void SetOffAttack() => anim.SetBool("Attack", false);
    public void Shooting()
    {
        GameObject bulletObj = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        
        if (isDead)
        {
            AudioManager.instance.StopSFX(0);
        }
        directionOfPlayer = Vector3.zero;
        AudioManager.instance.PlayerSFX(0);
        bulletScript.SetOwner(gameObject);
        bulletScript.SetTarget(targetEnemy);
        if (isGetGift)
        {
            bulletScript.isRotate = true;
            StartCoroutine(ScaleBullet(bulletObj, 3f, 130f, 0.3f)); // từ 39 lên 100 trong 1 giây
        }
        else
        {
            bulletObj.transform.localScale = new Vector3(39, 39, 39);
            bulletScript.isRotate = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet2"))
        {
            AudioManager.instance.isPlayerBGM = false;
            AudioManager.instance.PlayerSFX(0);
            dead1.SetActive(true);
            UIManager.instance.StartDead();
            anim.SetBool("Death", true);
            Harmmer.SetActive(false);
            isDead = true;
            PlayerPrefs.SetInt("coinMoney", coinMoney);
        }

        if (collision.gameObject.CompareTag("Gift"))
        {
            isGetGift = true;
            Destroy(collision.gameObject);
            radiusAttackOfPlayer = 8f;
            DrawCircle circle = GetComponentInChildren<DrawCircle>();
            if(circle != null)
            {
                circle.radius = 8f;
                circle.DrawCircleUnderFeet();
            }
        }
    }
    private IEnumerator ScaleBullet(GameObject bullet, float startScale, float endScale, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            if (bullet == null) yield break; // nếu bullet bị hủy → thoát Coroutine
            time += Time.deltaTime;
            float scale = Mathf.Lerp(startScale, endScale, time / duration);
            bullet.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        if (bullet != null)
            bullet.transform.localScale = new Vector3(endScale, endScale, endScale);
    }

    public void DestroyPlayer()
    {
        gameObject.SetActive(false);
    }
    public void UpLevel()
    {
        if(countAttack >= 1)
        {
            effectLevelUp.SetActive(true);
            transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            UIManager.instance.up = 4.7f;
            isLevelUp = true;
        }
    }
    public void SetDeufalt()
    {
        if (isGetGift)
        {
            isGetGift = false;
            radiusAttackOfPlayer = 5f;
            DrawCircle circle = GetComponentInChildren<DrawCircle>();
            if (circle != null)
            {
                circle.radius = 5f;
                circle.DrawCircleUnderFeet();
            }
            bullet1.transform.localScale = new Vector3(39, 39, 39);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusAttackOfPlayer);
    }
}