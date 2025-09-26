using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [Header("------------------Enemy Prefabs------------------")]
    public GameObject _enemyPrefabs;

    [Header("------------------Spawn Timing Settings------------------")]
    public float miniumSpawnTime;
    public float maxiumSpawnTime;

    [Header("------------------Spawn Control------------------")]
    public int maxEnemyCount; // số lượng tối đa enemy
    private float _timeUnitSpawn;

    [Header("------------------Runtime Enemy List------------------")]
    private List<Enemy> _enemyList = new List<Enemy>(); // danh sách quản lý enemy
    private void Awake(){
        SetTimeUnit();      // Khởi tạo thời gian spawn ngẫu nhiên ban đầu
    }

    private void Update(){
        _timeUnitSpawn -= Time.deltaTime;
        // Nếu hết thời gian chờ và số lượng enemy hiện tại < giới hạn cho phép
        if (_timeUnitSpawn < 0 && _enemyList.Count < maxEnemyCount){
            Vector3 spawnPos = transform.position +
                               new Vector3(Random.Range(-40, 40), 0, Random.Range(-25, 40));     // Random vị trí spawn quanh vị trí gốc của SpawnEnemy
            //GameObject enemy = Instantiate(_enemyPrefabs, spawnPos, Quaternion.identity);        // Tạo enemy mới và thêm vào danh sách quản lý
            Enemy enemy = Instantiate(_enemyPrefabs, spawnPos, Quaternion.identity).GetComponent<Enemy>();
            GameController.instance.enemies.Add(enemy);
            _enemyList.Add(enemy);
            SetTimeUnit();      // Reset lại thời gian chờ cho lần spawn tiếp theo
        }
    }

    public void SetTimeUnit()
    {
        _timeUnitSpawn = Random.Range(miniumSpawnTime, maxiumSpawnTime);
    }
}
