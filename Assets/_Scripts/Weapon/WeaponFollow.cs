using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponFollow : MonoBehaviour
{
    [SerializeField] private Transform HandPlayer;
    private MeshFilter mesh;
    public float x, y, z;
    public bool isCheckOne;

    public Vector3 rotationOffset;  // offset xoay
    private void Start()
    {
        mesh = GetComponent<MeshFilter>();
        
    }
    private void Update()
    {
        if (mesh.mesh.name == "Plane Instance")
        {
            transform.position = new Vector3(HandPlayer.transform.position.x + x, HandPlayer.transform.position.y + y, HandPlayer.transform.position.z + z);
        }  
    }
}
