using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponFollow : MonoBehaviour
{
    [SerializeField] private Transform HandPlayer;
    private MeshFilter mesh;
    public float x, y, z;
    private void Start()
    {
        mesh = GetComponent<MeshFilter>();
    }
    private void Update()
    {
        //Debug.Log("mesh.name" + mesh.mesh.name);
        if (mesh.mesh.name == "Plane Instance")
        {
            Debug.Log("mesh.name" + mesh.mesh.name);
            
            transform.position = new Vector3(HandPlayer.transform.position.x + x, HandPlayer.transform.position.y + y, HandPlayer.transform.position.z + z);
            transform.rotation = Quaternion.Euler(
        transform.eulerAngles.x,
        -106.6f,
        transform.eulerAngles.z
    );
        }
        else
        {
            transform.position = new Vector3(HandPlayer.transform.position.x, HandPlayer.transform.position.y, HandPlayer.transform.position.z);
        }
    }
}
