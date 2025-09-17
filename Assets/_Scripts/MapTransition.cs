using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    public Transform player;
    public Transform newPlayerPosition;
    public GameObject informationEnemyAlive;
    public GameObject setting;
    public GameObject enemy;
    public GameObject spawnEnemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DoTransition());
        }
    }

    private IEnumerator DoTransition()
    {
        // Tối màn hình
        yield return StartCoroutine(ScreenFader.instance.FadeOut(1f));

        // Dịch chuyển enemy sang map 2
        player.position = newPlayerPosition.position;
        informationEnemyAlive.SetActive(true);
        setting.SetActive(true);
        enemy.SetActive(true);
        spawnEnemy.SetActive(true);

        // Sáng màn hình
        yield return StartCoroutine(ScreenFader.instance.FadeIn(1f));
    }
}
