using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float moveSpeed = 2f;     // tốc độ bay lên
    public float duration = 1f;      // thời gian tồn tại
    public Vector3 moveDirection = new Vector3(0, 1, 0); // bay lên trên
    public TextMeshProUGUI textMesh;
    private Color startColor;
    public Transform followTarget;  // gán player khi spawn
    private Vector3 offset = new Vector3(0, 2f, 0);
    private float timer;

    private void Awake()
    {
        startColor = textMesh.color;
    }

    public void Setup(string text, Color color)
    {
        textMesh.text = text;
        textMesh.color = color;
        startColor = color;
    }

    void Update()
    {
        // luôn nhìn về camera
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);

        // di chuyển text
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // giảm dần alpha theo thời gian
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(startColor.a, 0, timer / duration);
        textMesh.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        // hết thời gian thì xóa
        if (timer >= duration)
        {
            Destroy(gameObject);
        }
    }
}
