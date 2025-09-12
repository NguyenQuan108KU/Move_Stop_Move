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

        int spawnedCount = maxEnemies - countEnemy; // số enemy đã spawn

        GameObject enemy = null;

        if (spawnedCount < 12) // 12 con đầu
        {
            int indexEnemy = Random.Range(0, 2); // 0 hoặc 1
            enemy = Instantiate(enemyPrefabList[indexEnemy], spawnPos, Quaternion.identity);
        }
        else if (spawnedCount < 18) // 6 con tiếp theo (12 -> 17)
        {
            int indexEnemy = Random.Range(2, 4); // 2 hoặc 3
            enemy = Instantiate(enemyPrefabList[indexEnemy], spawnPos, Quaternion.identity);
        }
        else if (spawnedCount < 20) // 2 con cuối (18 -> 19)
        {
            enemy = Instantiate(enemyPrefabList[4], spawnPos, Quaternion.identity);
        }

        if (enemy != null)
        {
            currentEnemies.Add(enemy); // nhớ add vào list quản lý
        }
    }

    Vector3 GetRandomNavmeshPosition(Vector3 center, float minDist, float maxDist)
    {
        Vector3 randomPos;      // Vị trí ngẫu nhiên trong sphere
        NavMeshHit hit;         // Kết quả trả về của NavMesh.SamplePosition
        int attempts = 0;       // Đếm số lần thử

        // Lặp để tìm vị trí hợp lệ
        do{
            // Lấy 1 điểm ngẫu nhiên trong sphere bán kính maxDist, dịch so với center
            randomPos = Random.insideUnitSphere * maxDist + center;
            attempts++;
            // Nếu thử quá 30 lần mà chưa có vị trí phù hợp thì dừng
            if (attempts > 30) break;
        } while (Vector3.Distance(center, randomPos) < minDist);
        // Kiểm tra nếu điểm quá gần center (< minDist) thì random lại

        // Thử lấy vị trí gần nhất trên NavMesh quanh randomPos
        if (NavMesh.SamplePosition(randomPos, out hit, maxDist, NavMesh.AllAreas)){
            return hit.position;    // Trả về vị trí đã "snap" lên NavMesh
        }
        return center;          // Nếu thất bại → trả về vị trí trung tâm
    }
}
