using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("------------------Player Components------------------")]
    public Animator anim;                  // Animator của nhân vật
    public Joystick joystick;             // Joystick điều khiển nhân vật

    [Header("------------------Bullet------------------")]
    public GameObject bulletPrefabs;      // Prefabs của viên đạn
    public Transform firingTransform;     // Vị trí viên đạn được bắn ra

    [Header("------------------Move Info------------------")]
    public float moveSpeedOfPlayer;      // Tốc độ di chuyển của nhân vật
    private Vector3 directionOfPlayer;   // Hướng di chuyển của nhân vật dựa trên joystick

    [Header("------------------Radius------------------")]
    public float radiusAttackOfPlayer;  // Bán kính vòng tròn phát hiện Enemy của Player

    public GameObject weaponOfPlayer;   // Vũ khí của nhân vật
    private Transform targetEnemy;      // Vị trí của Enemy
    public bool isAttack = false;       // Cho biết xem có tấn công hay không
    public int pointOfPlayerDefault;    // Điểm của người chơi
    private Enemy enemyCurrent;                            
    public bool isDead = false;         // Kiểm tra xem nhân vật đã chết hay chưa
    public int coinMoney;               // Tiền của người chơi

    [Header("------------------Change Weapon------------------")]
    public WeaponDatabase weaponData;
    public GameObject weaponChoose;
    public Bullet bullet1;              // Viên đạn của người chơi
    private int indexWeapon;
    public int indexMaterial;
    public DrawCircle circleAttack;     //Vòng tấn công của nhân vật 
    public GameObject effectLevelUp;
    public MeshRenderer weaponRenderer;
    public MeshFilter weaponMeshFilter;
    public MeshRenderer bulletRenderer;
    public MeshFilter bulletMeshFilter;
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
    private bool hasPlayedLevelUp = false;
    public GameObject popupLose;
    public void Init()
    {
        pointOfPlayerDefault = 0;
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
        SetWeaponOfPlayer();
    }
    //void Update()
    //{
    //    if (isDead) return;
    //    PlayerMove();
    //    AttackTrigle();
    //    UpLevel();
    //}
    //Thay đổi vũ khí
     public void SetWeaponOfPlayer(){
        indexWeapon = PlayerPrefs.GetInt("SelectOption");     //Lấy index của vũ khí 
        indexMaterial = PlayerPrefs.GetInt("MaterialOfWeapon" + indexWeapon);       //Lấy index của loại vũ khí 
        MeshRenderer meshRenderer = weaponRenderer;
        MeshRenderer meshRendererOfButton = bulletRenderer;
        Material[] mats = meshRenderer.materials;
        Material[] matsOfButton = meshRendererOfButton.sharedMaterials;
        string idWeapon = DataManager.Ins.gameSave.idWeapon;                // Lấy ID vũ khí hiện tại từ gameSave
        for (int i = 0; i < weaponData.weapon.Count(); i++){                // Duyệt danh sách vũ khí để tìm vũ khí khớp với idWeapon
            if (weaponData.weapon[i].index == idWeapon){
                // Gán mesh của vũ khí cho player và bullet
                weaponMeshFilter.mesh = weaponData.weapon[i].meshWeapon;
                bulletMeshFilter.mesh = weaponData.weapon[i].meshWeapon;

                // Thay đổi materials của vũ khí và bullet
                for (int j = 0; j < weaponData.listOfMaterials[indexWeapon].materialOfHammer[indexMaterial].materials.Length; j++){
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
    public void SetPantOfPlayer()
    {
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
    public void SetShieldOfPlayer(){
        string shieldName = DataManager.Ins.gameSave.idShield; // Lấy ID khiên đã lưu của người chơi

        // Kiểm tra xem shieldName có hợp lệ không
        if (!string.IsNullOrEmpty(shieldName)){
            // Xóa tất cả khiên cũ đang gắn trên shieldAnchor
            foreach (Transform child in shieldAnchor.transform){
                Destroy(child.gameObject); // Hủy từng khiên con
            }

            // Duyệt danh sách tất cả khiên trong shieldData
            for (int i = 0; i < shieldData.shields.Length; i++){
                // Nếu tìm thấy khiên có index trùng với shieldName
                if (shieldData.shields[i].index == shieldName){
                    // Sinh khiên mới và gắn vào shieldAnchor
                    Instantiate(shieldData.shields[i].shieldPrefab, shieldAnchor.transform);
                }
            }
        }
    }

    //Thay đổi skin của nhân vật 
    public void SetSkinOfPlayer(){
        string skinName = DataManager.Ins?.gameSave?.idSkin; // Lấy ID skin đã lưu của người chơi

        // Nếu skinName rỗng hoặc null thì thoát khỏi hàm
        if (string.IsNullOrEmpty(skinName)) return;

        // Xóa tất cả các item cũ trong list_anchorsOfSkin
        foreach (Transform anchor in list_anchorsOfSkin){
            foreach (Transform child in anchor){
                Destroy(child.gameObject); // Hủy từng child trong anchor
            }
        }

        // Duyệt danh sách skin để tìm skin có index trùng với skinName
        for (int i = 0; i < skinData.skin.Length; i++){
            // Nếu tìm thấy skin đúng
            if (skinData.skin[i].index == skinName){
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
        // Lấy giá trị đầu vào từ joystick
        directionOfPlayer.x = joystick.Horizontal;
        directionOfPlayer.z = joystick.Vertical;
        directionOfPlayer.y = 0;

        // Nếu joystick có nhập hướng
        if (directionOfPlayer.sqrMagnitude > 0.01f)
        {
            // Chuẩn hoá vector để hướng luôn có độ dài = 1
            weaponOfPlayer.SetActive(true);
            Vector3 moveDir = directionOfPlayer.normalized;

            // Di chuyển với vận tốc đồng đều
            transform.Translate(moveDir * moveSpeedOfPlayer * Time.deltaTime, Space.World);

            // Animation speed = 1 (luôn chạy, không phụ thuộc joystick)
            anim.SetFloat("Speed", 1f);

            // Xoay nhân vật mượt
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            anim.SetBool("Attack", false);
        }
        else
        {
            // Không di chuyển → anim về 0
            anim.SetFloat("Speed", 0f);
        }
    }
    // Hàm phát hiện enemy trong phạm vi tấn công
    public void AttackTrigle(){
        Collider[] colliders = Physics.OverlapSphere(transform.position, radiusAttackOfPlayer);     // Lấy tất cả các Collider xung quanh player trong bán kính radiusAttackOfPlayer
        Enemy firstEnemyDetected = null;         // Biến lưu enemy đầu tiên phát hiện

        // Duyệt tất cả collider để tìm enemy đầu tiên
        foreach (var hit in colliders){
            if (hit.CompareTag("Enemy")){
                firstEnemyDetected = hit.GetComponent<Enemy>();    // Lấy component Enemy từ collider
                break;                                             // Chỉ lấy enemy đầu tiên, dừng vòng lặp
            }
        }
        // Nếu tìm thấy enemy
        if (firstEnemyDetected != null){

            // Nếu enemy cũ khác enemy mới, tắt hiển thị enemy cũ
            if (enemyCurrent != null && enemyCurrent != firstEnemyDetected){
                enemyCurrent.circleTargetEnemy.SetActive(false); // Tắt enemy cũ nếu khác
            }
            enemyCurrent = firstEnemyDetected;             // Cập nhật enemy hiện tại
            enemyCurrent.circleTargetEnemy.SetActive(true);      // Bật hiển thị enemy mới

            // Nếu player không di chuyển
            if (directionOfPlayer.sqrMagnitude < 0.001f){
                targetEnemy = enemyCurrent.transform;
                //weaponOfPlayer.SetActive(true);
                anim.SetBool("Attack", true);
                Vector3 directionEnemy = targetEnemy.position - transform.position;    // Tính toán hướng quay về target enemy
                directionEnemy.y = 0;      // Giữ chiều cao không đổi
                Quaternion toRotation = Quaternion.LookRotation(directionEnemy);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);      // Quay player về hướng enemy một cách mượt mà
            }
            else{
                anim.SetBool("Attack", false);      // Nếu player đang di chuyển, tắt animation tấn công
            }
        }
        else{

            // Nếu không còn enemy nào trong bán kính
            if (enemyCurrent != null){
                enemyCurrent.circleTargetEnemy.SetActive(false);   // Tắt hiển thị enemy cũ
                enemyCurrent = null;                         // Reset enemy hiện tại
            }
        }
    }

    //Hàm bắn Enemy. Hàm này gọi trong event của animation
    public void Shooting(){
        weaponOfPlayer.SetActive(false);
        GameObject bulletObj = Instantiate(bulletPrefabs, firingTransform.position, Quaternion.identity);   // Tạo một viên đạn mới từ prefab tại vị trí firingTransform
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();     // Lấy component Bullet từ viên đạn vừa tạo
        directionOfPlayer = Vector3.zero;                           // Reset hướng di chuyển của player (hoặc hướng bắn) về Vector3.zero
        bulletScript.SetOwner(gameObject);                          // Thiết lập owner của viên đạn là chính player (tránh tự chết do viên đạn của mình)
        bulletScript.SetTarget(targetEnemy);                        // Thiết lập target của viên đạn là enemy hiện tại
        AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Attack);

        // Nếu player đang có gift (ăn được quà)
        if (isGetGift){
            bulletScript.isOffRotate = true;                           // Bật chế độ viên đạn không xoay
            StartCoroutine(ScaleBullet(bulletObj, 3f, 130f, 0.3f));    // Bắt đầu tăng scale viên đạn từ kích thước ban đầu lên to hơn 
        }
        else{
            bulletObj.transform.localScale = new Vector3(39, 39, 39);  // Nếu không có gift, đặt scale viên đạn cố định
            bulletScript.isOffRotate = false;                          // Tắt để viên đạn xoay như bình thường
        }
        if(isGetGift)
            StartCoroutine(AutoSetDefaultBullet(1f));
    }
    private IEnumerator AutoSetDefaultBullet(float delay)
    {
        yield return new WaitForSeconds(delay);
        // gọi reset từ player
        SetBulletPlayerDeufalt();
    }
    //Hàm thay đổi kích thước bullet mượt mà theo thời gian.
    private IEnumerator ScaleBullet(GameObject bullet, float startScale, float endScale, float duration){
        float time = 0f;
        while (time < duration){
            if (bullet == null) yield break; // nếu bullet bị hủy → thoát Coroutine
            time += Time.deltaTime;
            float scale = Mathf.Lerp(startScale, endScale, time / duration);            // Tính scale hiện tại
            bullet.transform.localScale = new Vector3(scale, scale, scale);             // Áp dụng scale cho bullet (uniform scale cả 3 trục)
            yield return null;
        }
        if (bullet != null)
            bullet.transform.localScale = new Vector3(endScale, endScale, endScale);    // Đảm bảo scale cuối cùng chính xác bằng endScale 
    }
    
    //Hàm tăng level cho player
    public void UpLevel(){

        // Kiểm tra điều kiện để nâng cấp
        if (pointOfPlayerDefault >= 3 && !hasPlayedLevelUp){
            AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Level_Up);
            hasPlayedLevelUp = true;                                    // Đánh dấu đã phát Level Up để không phát lại nhiều lần
            effectLevelUp.SetActive(true);      
            transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);       // Tăng kích thước player
            //GameController.instance.uiManager.up = 4.7f;                               // Cập nhật UI
            isLevelUp = true;                                           // Đánh dấu trạng thái player đang Level Up
            radiusAttackOfPlayer = 6.5f;  //Tăng phạm vi tấn công 
            circleAttack.radius = 6.5f;             
            circleAttack.DrawCircleUnderFeet();
        }
    }

    //Hàm này set lại kích thước về ban đầu  của player sau khi ăn quà xong 
    public void SetBulletPlayerDeufalt(){
        if (isGetGift){   
            isGetGift = false;          // Reset trạng thái gift để có thể ăn lại quà 
            radiusAttackOfPlayer = 5f;  // Đặt bán kính tấn công của player về mặc định
            if (circleAttack != null){
                circleAttack.radius = 5f;             // Reset bán kính hiển thị trên DrawCircle
                circleAttack.DrawCircleUnderFeet();   // Vẽ lại vòng tròn dưới chân player
            }
            bullet1.transform.localScale = new Vector3(39, 39, 39);   // Reset kích thước viên đạn về mặc định
        }
    }

    //Hàm vẽ màu cho Gizmos
    private void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusAttackOfPlayer);
    }
    public void SetOffAttack()
    {
        anim.SetBool("Attack", false);
        weaponOfPlayer.SetActive(true);
    }
    public void DestroyPlayer() => gameObject.SetActive(false);
    private void OnCollisionEnter(Collision collision){
        //Kiểm tra va chạm với Bullet của enemy
        if (collision.gameObject.CompareTag("Bullet2")){
            isDead = true;
            AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Lose);
            //Instantiate(popupLose, transform.position, Quaternion.identity);   //Sinh Popup Lose 
            anim.SetBool("Death", true);                                       // Kích hoạt animation chết
            weaponOfPlayer.SetActive(false);                                          //Tắt vũ khí của player khi ném            
            PlayerPrefs.SetInt("coinMoney", coinMoney);                        //Lưu tiền của player
            Destroy(collision.gameObject);
        }

        //Kiểm tra va chạm khi ăn quà 
        if (collision.gameObject.CompareTag("Gift")){
            AudioManager.Ins.PlaySoundEffect(SoundData.SoundName.Get_Gift);
            isGetGift = true;                           // Đánh dấu trạng thái nhận gift
            Destroy(collision.gameObject);              // Hủy gift khỏi scene
            radiusAttackOfPlayer = 8f;                  // Tăng phạm vi tấn công

            // Cập nhật vòng tròn hiển thị bán kính tấn công
            if(circleAttack != null){
                circleAttack.radius = 8f;                     //Tăng bán kính vòng to hơn
                circleAttack.DrawCircleUnderFeet();           // Vẽ lại vòng tròn
            }
        }
    }
}