using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGift : MonoBehaviour
{
    public ObjectPooling pooling;
    private float minTime;
    public float maxTime;
    private void Start()
    {
        minTime = maxTime;
    }

    private void Update()
    {
        minTime -= Time.deltaTime;
        if(minTime <= 0)
        {
            minTime = maxTime;
            CreateGift();
        }
    }
    void CreateGift()
    {
        GameObject gift = pooling.GetObject();

        float randomX = Random.Range(-40f, 40f);
        float randomZ = Random.Range(-40f, 40f);

        Vector3 randomPosition = new Vector3(randomX, 10f, randomZ);

        gift.transform.position = randomPosition;
    }
}
