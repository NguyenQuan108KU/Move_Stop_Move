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
    public int maxEnemies = 13; // Số lượng enemy tối đa
    public int totalEnemiesToSpawn = 15;   // Tổng số enemy cần spawn (bao gồm boss)
    public int countEnemy = 30;

    [Header("Spawn Radius")]
    public float minDistance = 8f;
    public float maxDistance = 13f;

    [Header("Spawn Time")]
    public float minSpawnInterval = 2f;
    public float maxSpawnInterval = 4f;

    public int spawnedTotal = 0;  // tổng số enemy đã spawn


    private List<GameObject> currentEnemies = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine()); // chỉ gọi 1 lần ở Start
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (spawnedTotal < totalEnemiesToSpawn)
        {
            SpawnEnemy();
            countEnemy -= 1;

            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);
        }
    }

    void SpawnEnemy()
    {
        Vector3 spawnPos = GetRandomNavmeshPosition(player.position, minDistance, maxDistance);
        Zombie enemy = null;

        // Tính hướng nhìn về player
        Quaternion lookRotation = Quaternion.LookRotation(player.position - spawnPos);

        if (spawnedTotal < totalEnemiesToSpawn - 1)  // 14 con đầu
        {
            int indexEnemy = Random.Range(0, 2); // random enemy thường
            enemy = Instantiate(enemyPrefabList[indexEnemy], spawnPos, lookRotation).GetComponent<Zombie>();
            ZombieCityController.instance.zombies.Add(enemy);
        }
        else if (spawnedTotal == totalEnemiesToSpawn - 1) // Con cuối cùng (Boss)
        {
            enemy = Instantiate(enemyPrefabList[4], spawnPos, lookRotation).GetComponent<Zombie>();
            ZombieCityController.instance.zombies.Add(enemy);
        }

        if (enemy != null)
        {
            spawnedTotal++;   // tăng số đã spawn lên
        }
    }


    Vector3 GetRandomNavmeshPosition(Vector3 center, float minDist, float maxDist)
    {
        NavMeshHit hit;
        int attempts = 0;
        Vector3 finalPos = center;

        float roadWidth = 2f; // chiều rộng nửa đường (±2)

        while (attempts < 30)
        {
            // random góc và khoảng cách
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(minDist, maxDist);

            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad));
            Vector3 randomPos = center + dir * distance;

            // Giữ enemy trong khoảng X hẹp (±roadWidth)
            randomPos.x = Mathf.Clamp(randomPos.x, center.x - roadWidth, center.x + roadWidth);

            // Check vị trí trên NavMesh
            if (NavMesh.SamplePosition(randomPos, out hit, 2f, NavMesh.AllAreas))
            {
                if (Vector3.Distance(center, hit.position) >= minDist)
                {
                    finalPos = hit.position;
                    break;
                }
            }
            attempts++;
        }

        return finalPos;
    }


}
