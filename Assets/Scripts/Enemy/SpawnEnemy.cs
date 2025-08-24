using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefabs;
    [SerializeField] private float miniumSpawnTime;
    [SerializeField] private float maxiumSpawnTime;
    [SerializeField] private int maxEnemyCount; // số lượng tối đa enemy

    private float _timeUnitSpawn;
    private List<GameObject> _enemyList = new List<GameObject>(); // danh sách quản lý enemy

    private void Awake()
    {
        SetTimeUnit();
    }

    private void Update()
    {
        _timeUnitSpawn -= Time.deltaTime;

        // Xóa enemy null (đã chết/destroy)
        _enemyList.RemoveAll(enemy => enemy == null);

        if (_timeUnitSpawn < 0 && _enemyList.Count < maxEnemyCount)
        {
            // Tạo enemy mới
            Vector3 spawnPos = GameManager.instance.playerController.transform.position +
                               new Vector3(Random.Range(-35, 35), 0, Random.Range(-35, 35));

            GameObject enemy = Instantiate(_enemyPrefabs, spawnPos, Quaternion.identity);
            _enemyList.Add(enemy);

            SetTimeUnit();
        }
    }

    public void SetTimeUnit()
    {
        _timeUnitSpawn = Random.Range(miniumSpawnTime, maxiumSpawnTime);
    }
}
