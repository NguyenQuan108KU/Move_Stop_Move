using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnZombie : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public Transform player;
    public int maxEnemies = 10; // Số lượng enemy tối đa

    [Header("Spawn Radius")]
    public float minDistance = 5f;
    public float maxDistance = 15f;

    [Header("Spawn Time")]
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 6f;

    private List<GameObject> currentEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (true)
        {
            // Xóa các enemy bị hủy khỏi danh sách
            currentEnemies.RemoveAll(e => e == null);

            if (currentEnemies.Count < maxEnemies)
            {
                SpawnEnemy();
            }

            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos = GetRandomNavmeshPosition(player.position, minDistance, maxDistance);
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        currentEnemies.Add(enemy);
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
