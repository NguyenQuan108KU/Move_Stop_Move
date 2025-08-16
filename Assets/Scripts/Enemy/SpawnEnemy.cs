using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefabs;
    [SerializeField] private float miniumSpawnTime;
    [SerializeField] private float maxiumSpawnTime;
    private float _timeUnitSpawn;
    private void Awake()
    {
        SetTimeUnit();
    }
    private void Update()
    {
        _timeUnitSpawn -= Time.deltaTime;
        if(_timeUnitSpawn < 0)
        {
            Instantiate(_enemyPrefabs, GameManager.instance.playerController.transform.position + new Vector3(Random.Range(-35, 35), 0, Random.Range(-35, 35)), Quaternion.identity);
            SetTimeUnit();
        }
    }
    public void SetTimeUnit()
    {
        _timeUnitSpawn = Random.Range(miniumSpawnTime, maxiumSpawnTime);
    }
}
