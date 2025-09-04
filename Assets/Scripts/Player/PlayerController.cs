using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public Animator anim;
    public Joystick joystick;

    [Header("------------------Bullet------------------")]
    public GameObject bulletPrefabs;   //Prefabs của viên đạn
    public Transform firingTransform;  //Nơi viên đạn được bắn ra

    [Header("------------------Move Info------------------")]
    public float moveSpeedOfPlayer;    //Tốc độ di chuyển của nhân vật
    private Vector3 directionOfPlayer;        // Hướng di chuyển của nhân vật dựa trên joystick

    [Header("------------------Radius------------------")]
    public float radiusAttackOfPlayer;  //Bán kính vòng tròn phát hiện Enemy của Player

    public GameObject Harmmer;          //Vũ khí của nhân vật
    private Transform targetEnemy;      //Vị trí của Enemy
    public bool isAttack = false;       // Cho biết xem có tấn công hay không
    public float attackDuration = 1f;   // Thời gian duy trì trạng thái tấn công
    public int point;                   //Điểm của người chơi
    private Enemy enemyCurrent;                            
    public bool isDead = false;   //Kiểm tra xem nhân vật đã chết hay chưa
    public GameObject dead1;
    public int coinMoney;       //Tiền của người chơi

    //Thay đổi vũ khi của player
    [Header("Change Weapon")]
    public WeaponDatabase weaponDB;
    public Test test;
    public GameObject weaponChoose;
    public Bullet bullet1;              //Viên đạn của người chơi
    private int indexWeapon;
    [SerializeField] private int indexMaterial;
    public int countAttack;
    public GameObject effectLevelUp;
    [Header("------------------Change Pants------------------")]
    [SerializeField] private int indexPants;
    [SerializeField] private ListPants listPants;
    [SerializeField] private GameObject pantsOdPlayer;

    [Header("------------------Change Hats------------------")]
    [SerializeField] private int indexHats;
    [SerializeField] private HATS hatOfPlayer;

    [Header("------------------Change Protect------------------")]
    [SerializeField] private int indexProtect;
    [SerializeField] private Protect protectOfPlayer;

    [Header("------------------Change Clothes Player------------------")]
    [SerializeField] private int indexClothes;
    [SerializeField] private ClothesSet[] listClothes;
    [SerializeField] private GameObject initialShadingOfPlayer;
    [SerializeField] private GameObject PantsOfPlayer;

    public bool isGetGift = false;
    public bool isLevelUp = false;
    void Start()
    {
        point = 0;
        coinMoney = PlayerPrefs.GetInt("coinMoney");
        isGetGift = false;
    }
    void Update()
    {
        if (isDead) return;
        PlayerMove();
        AttackTrigle();
        //Thay doi quan ao, vu khi
        ChangeWepon();
        changePants();
        changeHats();
        changeProtect();
        changeClothesPlayer();
        //Len level
        UpLevel();
    }
    //Thay đổi vũ khí
    void ChangeWepon()
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
    //Thay đổi giày 
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
    //Thay đổi mũ
    void changeHats()
    {
        //indexHats = PlayerPrefs.GetInt("IndexHat");
        indexHats = PlayerPrefs.GetInt("SlectHat", -1); 
        if(indexHats == -1)
        {
            hatOfPlayer.games[6].SetActive(true);
        }
        for (int i = 0; i < hatOfPlayer.games.Count(); i++)
        {
            if(indexHats == i)
            {
                hatOfPlayer.games[i].SetActive(true);
            }
            else{
                hatOfPlayer.games[i].SetActive(false);
            }
        }
    }
    //Thay đổi khiên
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
    //Thay đổi full set 
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

    //Hàm di chuyển nhân vật
    private void PlayerMove(){
        //Lấy hướng di chuyển từ joystick
        directionOfPlayer.x = joystick.Horizontal;
        directionOfPlayer.z = joystick.Vertical;
        directionOfPlayer.y = 0;

        transform.Translate(directionOfPlayer * moveSpeedOfPlayer * Time.deltaTime, Space.World);   //Di chuyển nhân vật bằng Translate
        anim.SetFloat("Speed", directionOfPlayer.sqrMagnitude);    //Chuyển sang animation di chuyển
        if (directionOfPlayer.sqrMagnitude > 0.01f){               //Nếu nhân vật đang có hướng di chuyển thì xoay theo hướng đó 
            Quaternion toRotation = Quaternion.LookRotation(directionOfPlayer, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }
    }

    //Hàm phát hiện enemy 
    public void AttackTrigle(){
        Collider[] colliders = Physics.OverlapSphere(transform.position, radiusAttackOfPlayer);
        Enemy firstEnemyDetected = null;
        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                firstEnemyDetected = hit.GetComponent<Enemy>();
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
            if (directionOfPlayer.sqrMagnitude == 0.0f)
            {
                targetEnemy = enemyCurrent.transform;
                anim.SetBool("Attack", true);
                Vector3 directionEnemy = targetEnemy.position - transform.position;
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