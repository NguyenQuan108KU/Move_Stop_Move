using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public GameObject giftPrefabs;
    public int poolSize;
    public Queue<GameObject> pool;

    private void Awake()
    {
        pool = new Queue<GameObject>();
        for(int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(giftPrefabs);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }
    public GameObject GetObject()
    {
        while (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            if (obj != null)  //kiểm tra xem obj còn tồn tại không
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // Nếu pool rỗng hoặc obj null -> tạo mới
        GameObject newObj = Instantiate(giftPrefabs);
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj != null)   // tránh null reference
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

}
