using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponFollow : MonoBehaviour
{
    [SerializeField] private Transform HandPlayer;
    private MeshFilter mesh;
    private Transform transform_Basic;
    public float x, y, z;

    private Vector3 originalOffset;
    private Quaternion originalRotation;
    private void Start()
    {
        mesh = GetComponent<MeshFilter>();
        // Lưu offset và rotation so với HandPlayer lúc khởi tạo
    }
    private void Update()
    {
        if (mesh.mesh.name == "Plane Instance")
        {
            Scene currentScene = SceneManager.GetActiveScene();
            int sceneIndex = currentScene.buildIndex;
            if (sceneIndex == 0)
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
