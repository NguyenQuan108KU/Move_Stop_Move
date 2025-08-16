using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    private void LateUpdate()
    {
        transform.position = new Vector3(playerTransform.transform.position.x, playerTransform.transform.position.y, playerTransform.transform.position.z);
    }
}
