using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnZombie : MonoBehaviour
{
    [Header("Enemy Settings")]
    public List<GameObject> enemyPrefabList;
    public GameObject enemyPrefab;
    public Transform player;
    public int maxEnemies = 20; // Số lượng enemy tối đa
    public int countEnemy = 30;

    [Header("Spawn Radius")]
    public float minDistance = 8f;
    public float maxDistance = 13f;

    [Header("Spawn Time")]
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 4f;

    private List<GameObject> currentEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine()); // chỉ gọi 1 lần ở Start
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (countEnemy > 0)
        {
            // dọn rác enemy đã chết
            currentEnemies.RemoveAll(e => e == null);

            // chỉ spawn khi chưa đủ maxEnemies
            if (currentEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
                countEnemy -= 1;
            }

            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos = GetRandomNavmeshPosition(player.position, minDistance, maxDistance);
        if (countEnemy == 24  || countEnemy == 28 || countEnemy == 27 || countEnemy == 26 || countEnemy == 22)
        {
            GameObject enemy = Instantiate(enemyPrefabList[2], spawnPos, Quaternion.identity);
        }
        else if (countEnemy == 2 || countEnemy == 1 || countEnemy == 7)
        {
            GameObject enemy = Instantiate(enemyPrefabList[3], spawnPos, Quaternion.identity);
        }
        else
        {
            int indexEnemy = Random.Range(0, 2);
            GameObject enemy = Instantiate(enemyPrefabList[indexEnemy], spawnPos, Quaternion.identity);
        }
    }

    Vector3 GetRandomNavmeshPosition(Vector3 center, float minDist, float maxDist)
    {
        Vector3 randomPos;
        NavMeshHit hit;
        int attempts = 0;

        do
        {
            randomPos = Random.insideUnitSphere * maxDist + center;
            attempts++;
            if (attempts > 30) break;
        } while (Vector3.Distance(center, randomPos) < minDist);

        if (NavMesh.SamplePosition(randomPos, out hit, maxDist, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return center;
    }
}
