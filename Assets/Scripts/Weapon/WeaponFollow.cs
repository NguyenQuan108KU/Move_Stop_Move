using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Scene currentScene = SceneManager.GetActiveScene();
            int sceneIndex = currentScene.buildIndex;
            if(sceneIndex == 0)
            {
                transform.position = new Vector3(HandPlayer.transform.position.x + x, HandPlayer.transform.position.y + y, HandPlayer.transform.position.z + z);

                transform.rotation = Quaternion.Euler(
            transform.eulerAngles.x,
            -106.5f,
            transform.eulerAngles.z);
            }
            else
            {
                transform.position = new Vector3(HandPlayer.transform.position.x + x, HandPlayer.transform.position.y + y, HandPlayer.transform.position.z + z);
            }
        }
        else
        {
            transform.position = new Vector3(HandPlayer.transform.position.x, HandPlayer.transform.position.y, HandPlayer.transform.position.z);
        }
    }
}
