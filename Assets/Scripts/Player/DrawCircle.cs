using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawCircle : MonoBehaviour
{
    public float radius = 10f;
    public int segments = 100;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;
        lineRenderer.positionCount = segments;

        DrawCircleUnderFeet();
    }

    public void DrawCircleUnderFeet()
    {
        Vector3[] points = new Vector3[segments];
        float angle = 0f;
        Vector3 center = transform.position; // vị trí player trong world

        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            points[i] = center + new Vector3(x, 0f, z);
            angle += 2 * Mathf.PI / segments;
        }

        lineRenderer.SetPositions(points);
    }
    void Update()
    {
        DrawCircleUnderFeet();
    }
}
