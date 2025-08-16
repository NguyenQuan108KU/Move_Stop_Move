using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DrawCircle : MonoBehaviour
{
    public float radius = 3f;
    public int segments = 100;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = segments;

        DrawCircleUnderFeet();
    }

    void DrawCircleUnderFeet()
    {
        Vector3[] points = new Vector3[segments];
        float angle = 0f;
        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            points[i] = new Vector3(x, 0f, z); 
            angle += 2 * Mathf.PI / segments;
        }

        lineRenderer.SetPositions(points);
    }
}
